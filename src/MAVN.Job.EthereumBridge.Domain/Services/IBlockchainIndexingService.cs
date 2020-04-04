using System.Threading.Tasks;

namespace MAVN.Job.EthereumBridge.Domain.Services
{
    public interface IBlockchainIndexingService
    {
        Task IndexUntilLastBlockAsync();
    }
}
