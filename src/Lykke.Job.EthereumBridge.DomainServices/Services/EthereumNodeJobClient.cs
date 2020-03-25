using System;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Job.EthereumBridge.Domain.Services;
using Nethereum.Contracts.Services;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Lykke.Job.EthereumBridge.DomainServices.Services
{
    public class EthereumNodeJobClient : IEthereumNodeJobClient
    {
        private readonly IEthApiContractService _ethApi;
        private readonly ISettingsService _settingsService;

        public EthereumNodeJobClient(IEthApiContractService ethApi, ISettingsService settingsService)
        {
            _ethApi = ethApi;
            _settingsService = settingsService;
        }

        public async Task<long> GetBlockNumberAsync()
        {
            var getBestExistingBlockNumber = await _ethApi.Blocks.GetBlockNumber.SendRequestAsync();
            return (long) getBestExistingBlockNumber.Value;
        }

        public Task<FilterLog[]> GetTransactionsLogs(BigInteger fromBlock, BigInteger toBlock)
        {
            return _ethApi.Filters.GetLogs.SendRequestAsync(new NewFilterInput
            {
                Address = new[]
                {
                    _settingsService.GetPublicBlockchainGatewayContractAddress().ToLowerInvariant(),
                    _settingsService.GetPublicBlockchainTokenContractAddress().ToLowerInvariant()
                },
                FromBlock = new BlockParameter(new HexBigInteger(fromBlock)),
                ToBlock = new BlockParameter(new HexBigInteger(toBlock)),
            });
        }

        public async Task<int?> GetTransactionStatus(string transactionHash)
        {
            var tx = await _ethApi.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);

            return (int?) tx?.Status?.Value;
        }

        public async Task<long> GetWalletBalanceAsync(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            var response = await _ethApi.GetBalance.SendRequestAsync(address);

            return (long) response.Value;
        }
    }
}
