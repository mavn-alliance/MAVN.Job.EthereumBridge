using System.Threading.Tasks;

namespace Lykke.Job.EthereumBridge.Domain.Services
{
    public interface IEthereumNodeServiceClient
    {
        Task<long> GetBlockNumberAsync();
    }
}
