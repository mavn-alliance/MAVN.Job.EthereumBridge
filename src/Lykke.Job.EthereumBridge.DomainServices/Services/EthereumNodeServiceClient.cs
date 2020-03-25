using System.Threading.Tasks;
using Lykke.Job.EthereumBridge.Domain.Services;
using Nethereum.Contracts.Services;

namespace Lykke.Job.EthereumBridge.DomainServices.Services
{
    public class EthereumNodeServiceClient : IEthereumNodeServiceClient
    {
        private readonly IEthApiContractService _ethApi;
        private readonly int _blockConfirmationLevel;

        public EthereumNodeServiceClient(IEthApiContractService ethApi, int blockConfirmationLevel)
        {
            _ethApi = ethApi;
            _blockConfirmationLevel = blockConfirmationLevel;
        }

        public async Task<long> GetBlockNumberAsync()
        {
            var getBestExistingBlockNumber = await _ethApi.Blocks.GetBlockNumber.SendRequestAsync();
            return (long) getBestExistingBlockNumber.Value - _blockConfirmationLevel;
        }
    }
}
