using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Falcon.Numerics;
using Lykke.Common.Log;
using Lykke.Job.EthereumBridge.Contract;
using Lykke.Job.EthereumBridge.Domain.Repositories;
using Lykke.Job.EthereumBridge.Domain.Services;
using Lykke.PublicBlockchain.Definitions;
using Lykke.RabbitMqBroker.Publisher;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;
using Nethereum.RPC.Eth.DTOs;

namespace Lykke.Job.EthereumBridge.DomainServices.Services
{
    public class BlockchainIndexingService : IBlockchainIndexingService
    {
        private readonly string _publicAccountLinkedEventSignature;
        private readonly string _publicAccountUnlinkedEventSignature;
        private readonly string _transferredFromInternalNetworkEventSignature;
        private readonly string _transferredToInternalNetworkEventSignature;
        private readonly string _seizeFromEventSignature;
        private readonly IEthereumNodeJobClient _ethereumNodeJobClient;
        private readonly IIndexingStateRepository _indexingStateRepository;

        private readonly IRabbitPublisher<EthereumWalletLinkingStatusChangeCompletedEvent>
            _walletLinkingStatusChangeCompletedPublisher;

        private readonly IRabbitPublisher<TransferToExternalCompletedEvent> _transferToExternalCompletedPublisher;
        private readonly IRabbitPublisher<TransferToInternalDetectedEvent> _transferToInternalDetectedPublisher;
        private readonly IRabbitPublisher<SeizeToInternalDetectedEvent> _seizeToInternalDetectedPublisher;
        private readonly EventTopicDecoder _eventTopicDecoder;
        private readonly int _blockConfirmationLevel;
        private readonly int? _blockBatchSize;
        private readonly ILog _log;

        private long? _lastIndexedBlock;

        public BlockchainIndexingService(
            IEthereumNodeJobClient ethereumNodeJobClient,
            IIndexingStateRepository indexingStateRepository,
            IRabbitPublisher<EthereumWalletLinkingStatusChangeCompletedEvent>
                walletLinkingStatusChangeCompletedPublisher,
            IRabbitPublisher<TransferToExternalCompletedEvent> transferToExternalCompletedPublisher,
            IRabbitPublisher<TransferToInternalDetectedEvent> transferToInternalDetectedPublisher,
            IRabbitPublisher<SeizeToInternalDetectedEvent> seizeToInternalDetectedPublisher, 
            int blockConfirmationLevel,
            ILogFactory logFactory,
            int? blockBatchSize = null)
        {
            _ethereumNodeJobClient = ethereumNodeJobClient;
            _indexingStateRepository = indexingStateRepository;
            _walletLinkingStatusChangeCompletedPublisher = walletLinkingStatusChangeCompletedPublisher;
            _transferToExternalCompletedPublisher = transferToExternalCompletedPublisher;
            _transferToInternalDetectedPublisher = transferToInternalDetectedPublisher;
            _seizeToInternalDetectedPublisher = seizeToInternalDetectedPublisher;
            _blockConfirmationLevel = blockConfirmationLevel;
            _blockBatchSize = blockBatchSize;
            _publicAccountLinkedEventSignature =
                $"0x{ABITypedRegistry.GetEvent<PublicAccountLinkedEventDTO>().Sha3Signature}";
            _publicAccountUnlinkedEventSignature =
                $"0x{ABITypedRegistry.GetEvent<PublicAccountUnlinkedEventDTO>().Sha3Signature}";
            _transferredFromInternalNetworkEventSignature =
                $"0x{ABITypedRegistry.GetEvent<TransferredFromInternalNetworkEventDTO>().Sha3Signature}";
            _transferredToInternalNetworkEventSignature =
                $"0x{ABITypedRegistry.GetEvent<TransferredToInternalNetworkEventDTO>().Sha3Signature}";
            _publicAccountUnlinkedEventSignature =
                $"0x{ABITypedRegistry.GetEvent<PublicAccountUnlinkedEventDTO>().Sha3Signature}";
            _seizeFromEventSignature =
                $"0x{ABITypedRegistry.GetEvent<SeizeFromEventDTO>().Sha3Signature}";
            _eventTopicDecoder = new EventTopicDecoder();
            _log = logFactory.CreateLog(this);
        }

        public async Task IndexUntilLastBlockAsync()
        {
            var context = Guid.NewGuid().ToString();

            _log.Info("Trying to index new block", context: context);

            var lastConfirmedBlockNumber = await _ethereumNodeJobClient.GetBlockNumberAsync() - _blockConfirmationLevel;

            _log.Info(
                $"Last confirmed block = {lastConfirmedBlockNumber}, using confirmation level = {_blockConfirmationLevel}",
                context: context);

            var lastIndexedBlockNumber = await GetLastBlockFromDbAsync();
            if (lastIndexedBlockNumber == lastConfirmedBlockNumber)
            {
                _log.Info(
                    $"Latest indexed block is the latest confirmed block, nothing to index. Block number {lastIndexedBlockNumber}",
                    context: context);
                return;
            }

            if (lastIndexedBlockNumber > lastConfirmedBlockNumber)
            {
                _log.Info(
                    $"Network last block {lastConfirmedBlockNumber} is lower than last indexed block {lastIndexedBlockNumber}. Probably, the node not synced yet.");
                return;
            }

            while (lastConfirmedBlockNumber > lastIndexedBlockNumber)
            {
                var nextBlockNumberToIndex = lastIndexedBlockNumber + 1;

                await IndexBlocksAsync(nextBlockNumberToIndex, lastConfirmedBlockNumber);

                lastIndexedBlockNumber = await GetLastBlockFromDbAsync();
                lastConfirmedBlockNumber = await _ethereumNodeJobClient.GetBlockNumberAsync() - _blockConfirmationLevel;
            }
        }

        private async Task IndexBlocksAsync(long blockNumber, long lastConfirmedBlockNumber)
        {
            var batchSize = _blockBatchSize ?? 1000;

            var processEventsTasks = new List<Task>();

            for (var i = blockNumber; i <= lastConfirmedBlockNumber; i += batchSize)
            {
                var batchTo = Math.Min(i + batchSize, lastConfirmedBlockNumber);

                var transactionLogs = await _ethereumNodeJobClient.GetTransactionsLogs(blockNumber, batchTo);

                var processLogsTask = ProcessEventLogs(transactionLogs);

                processEventsTasks.Add(processLogsTask);
            }

            await Task.WhenAll(processEventsTasks);

            await _indexingStateRepository.SaveLastIndexedBlockNumber(lastConfirmedBlockNumber);

            _lastIndexedBlock = lastConfirmedBlockNumber;

            _log.Info($"Blocks from [{blockNumber}] to [{lastConfirmedBlockNumber}] has been indexed.");
        }

        private async Task ProcessEventLogs(IEnumerable<FilterLog> logs)
        {
            foreach (var log in logs)
            {
                var topicsAsStrings = log.Topics
                    .Select(o => o.ToString())
                    .ToList();

                var firstTopic = topicsAsStrings[0];

                if (firstTopic == _publicAccountLinkedEventSignature)
                    await ProcessPublicAccountLinkedEvent(topicsAsStrings, log.Data);

                else if (firstTopic == _publicAccountUnlinkedEventSignature)
                    await ProcessPublicAccountUnlinkedEvent(topicsAsStrings, log.Data);

                else if (firstTopic == _transferredFromInternalNetworkEventSignature)
                    await ProcessTransferFromInternalNetworkEvent(topicsAsStrings, log.Data);

                else if (firstTopic == _transferredToInternalNetworkEventSignature)
                    await ProcessTransferToInternalNetworkEvent(topicsAsStrings, log.Data);
                
                else if (firstTopic == _seizeFromEventSignature)
                    await ProcessSeizeFromEvent(topicsAsStrings, log.Data);
            }
        }

        private Task ProcessPublicAccountLinkedEvent(IEnumerable<string> topics, string data)
        {
            var eventData = DecodeEvent<PublicAccountLinkedEventDTO>(topics, data);

            var evt = new EthereumWalletLinkingStatusChangeCompletedEvent
            {
                PrivateAddress = eventData.InternalAccount, PublicAddress = eventData.PublicAccount
            };

            return _walletLinkingStatusChangeCompletedPublisher.PublishAsync(evt);
        }

        private Task ProcessPublicAccountUnlinkedEvent(IEnumerable<string> topics, string data)
        {
            var eventData = DecodeEvent<PublicAccountUnlinkedEventDTO>(topics, data);

            var evt = new EthereumWalletLinkingStatusChangeCompletedEvent {PrivateAddress = eventData.InternalAccount};

            return _walletLinkingStatusChangeCompletedPublisher.PublishAsync(evt);
        }

        private Task ProcessTransferFromInternalNetworkEvent(IEnumerable<string> topics, string data)
        {
            var eventData = DecodeEvent<TransferredFromInternalNetworkEventDTO>(topics, data);

            var evt = new TransferToExternalCompletedEvent
            {
                PrivateAddress = eventData.InternalAccount,
                Amount = Money18.CreateFromAtto(eventData.Amount),
                PublicAddress = eventData.PublicAccount,
                Timestamp = DateTime.UtcNow,
                EventId = Guid.NewGuid().ToString()
            };

            return _transferToExternalCompletedPublisher.PublishAsync(evt);
        }

        private Task ProcessTransferToInternalNetworkEvent(IEnumerable<string> topics, string data)
        {
            var eventData = DecodeEvent<TransferredToInternalNetworkEventDTO>(topics, data);

            var evt = new TransferToInternalDetectedEvent
            {
                PrivateAddress = eventData.InternalAccount,
                Amount = Money18.CreateFromAtto(eventData.Amount),
                PublicAddress = eventData.PublicAccount,
                Timestamp = DateTime.UtcNow,
                OperationId = Guid.NewGuid().ToString(),
                PublicTransferId = eventData.PublicTransferId
            };

            return _transferToInternalDetectedPublisher.PublishAsync(evt);
        }

        private async Task ProcessSeizeFromEvent(IEnumerable<string> topics, string data)
        {
            var eventData = DecodeEvent<SeizeFromEventDTO>(topics, data);

            var evt = new SeizeToInternalDetectedEvent
            {
                OperationId = Guid.NewGuid().ToString(),
                Account = eventData.Account,
                Amount = Money18.CreateFromAtto(eventData.Amount),
                Reason = eventData.Reason,
                Timestamp = DateTime.UtcNow
            };

            await _seizeToInternalDetectedPublisher.PublishAsync(evt);

            _log.Warning("Seize from event published",
                context: $"account : {evt.Account}; amount: {evt.Amount}; reason: {evt.Reason}");
        }

        private T DecodeEvent<T>(IEnumerable<string> topics, string data) where T : class, new()
            => _eventTopicDecoder.DecodeTopics<T>(topics.Select(o => (object) o).ToArray(), data);

        private async Task<long> GetLastBlockFromDbAsync()
        {
            var getBestIndexedBlockNumber = _lastIndexedBlock
                                            ?? await _indexingStateRepository.GetLastIndexedBlockNumberAsync();

            return getBestIndexedBlockNumber ?? -1;
        }
    }
}
