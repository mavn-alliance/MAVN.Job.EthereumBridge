using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Service.QuorumTransactionSigner.Client;
using MAVN.Job.EthereumBridge.Domain.Repositories;
using MAVN.Job.EthereumBridge.Domain.Services;
using MAVN.Job.EthereumBridge.DomainServices.Extensions;
using MAVN.Persistence.PostgreSQL.Legacy;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.RPC.Eth.Services;
using Nethereum.Signer;

namespace MAVN.Job.EthereumBridge.DomainServices.Services
{
    public class OperationsService : IOperationsService
    {
        private readonly IEthApiTransactionsService _ethereumApi;
        private readonly IQuorumTransactionSignerClient _transactionSigner;
        private readonly IOperationsRepository _operationsRepository;
        private readonly INoncesRepository _noncesRepository;
        private readonly ITransactionRunner _transactionRunner;
        private readonly long _gasLimit;
        private readonly long _gasPrice;
        private readonly ILog _log;

        public OperationsService(
            IEthApiTransactionsService ethereumApi,
            IQuorumTransactionSignerClient transactionSigner,
            IOperationsRepository operationsRepository,
            INoncesRepository noncesRepository,
            ITransactionRunner transactionRunner,
            long gasLimit,
            long gasPrice,
            ILogFactory logFactory)
        {
            _ethereumApi = ethereumApi;
            _transactionSigner = transactionSigner;
            _operationsRepository = operationsRepository;
            _noncesRepository = noncesRepository;
            _transactionRunner = transactionRunner;
            _gasLimit = gasLimit;
            _gasPrice = gasPrice;
            _log = logFactory.CreateLog(this);
        }

        public async Task ExecuteOperationAsync(
            string operationId,
            string masterWalletAddress,
            string targetWalletAddress,
            string encodedData)
        {
            var (transactionData, transactionHash) = await _operationsRepository.GetOrCreateOperationAsync(operationId);

            if (transactionHash == null)
            {
                if (transactionData == null) // Transaction has been neither built, nor broadcasted
                    transactionData = await BuildTransactionAsync(operationId, masterWalletAddress, targetWalletAddress, encodedData);

                // Transaction has already been built, but has not been broadcasted yet
                var broadcastedData = await SendTransactionAsync(operationId, transactionData);

                await SetTransactionHash(operationId, broadcastedData.Item2);

                return;
            }

            //Transaction has already been built and broadcasted
            if (transactionData == null)
            {
                _log.Warning("Operation has hash, but has no data.", context: new { operationId });
                return;
            }

            _log.Info("Operation has already been built and broadcasted.", new { operationId, transactionHash });
        }

        private async Task SetTransactionHash(string operationId, string transactionHash)
        {
            try
            {
                await _operationsRepository.SetTransactionHashAsync(operationId, transactionHash);

                _log.Info("Transaction hash has been saved.", new {operationId, transactionHash});
            }
            catch (Exception e)
            {
                _log.Error(e, "Failed to save transaction hash.", new {operationId, transactionHash });

                throw;
            }
        }

        private async Task<string> BuildTransactionAsync(
            string operationId,
            string masterWalletAddress,
            string targetWalletAddress,
            string encodedData)
        {
            return await _transactionRunner.RunWithTransactionAsync(async txContext =>
            {
                var nonce = await _noncesRepository.GetNextNonceAsync(masterWalletAddress, txContext);

                var transaction = new Transaction(
                    to: targetWalletAddress,
                    amount: 0,
                    nonce: nonce,
                    gasPrice: _gasPrice,
                    gasLimit: _gasLimit,
                    data: encodedData
                );

                _log.Info
                (
                    "Transaction has been built.",
                    new { operationId, masterWalletAddress, nonce }
                );

                try
                {
                    var (v, r, s) = await _transactionSigner.SignTransactionAsync(masterWalletAddress, transaction.RawHash);

                    var signature = EthECDSASignatureFactory.FromComponents(r: r, s: s, v: v);

                    transaction.SetSignature(signature);

                    #region Logging

                    _log.Info
                    (
                        "Transaction has been signed.",
                        new { operationId, masterWalletAddress, nonce }
                    );

                    #endregion
                }
                catch (Exception e)
                {
                    #region Logging

                    _log.Error
                    (
                        e,
                        "Failed to sign transaction.",
                        new { operationId, masterWalletAddress, nonce }
                    );

                    #endregion

                    throw;
                }

                var transactionData = transaction.GetRLPEncoded().ToHex(true);

                try
                {
                    await _operationsRepository.SetTransactionDataAsync(
                        operationId,
                        transactionData,
                        txContext);

                    #region Logging

                    _log.Info
                    (
                        "Transaction data has been saved.",
                        new { operationId, masterWalletAddress, nonce }
                    );

                    #endregion
                }
                catch (Exception e)
                {
                    #region Logging

                    _log.Error
                    (
                        e,
                        "Failed to save transaction data.",
                        new { operationId, masterWalletAddress, nonce }
                    );

                    #endregion

                    throw;
                }

                return transactionData;
            });
        }

        public async Task RepostHangedOperations()
        {
            var timestampBeforeConsideredHanged = DateTime.UtcNow.AddMinutes(-10);

            var hangedOperations = await _operationsRepository.GetHandedOperationsIds(timestampBeforeConsideredHanged);

            foreach (var operation in hangedOperations)
            {
                // What will happen if the transaction succeeded and we try to repost it
                var (operationId, txHash) = await SendTransactionAsync(operation.OperationId, operation.TransactionData);
                await SetTransactionHash(operationId, txHash);
            }
        }

        private async Task<(string, string)> SendTransactionAsync(string operationId, string transactionData)
        {
            try
            {
                var transactionHash = await _ethereumApi.SendTransactionAsync(transactionData);

                _log.Info("Transaction has been broadcasted.", new { operationId, transactionHash });

                return (operationId, transactionHash);
            }
            catch (Exception e)
            {
                _log.Error(e, "Failed to broadcast transaction for operation.", new { operationId });

                throw;
            }
        }
    }
}
