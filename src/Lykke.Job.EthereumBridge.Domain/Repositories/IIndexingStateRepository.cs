using System.Threading.Tasks;

namespace Lykke.Job.EthereumBridge.Domain.Repositories
{
    public interface IIndexingStateRepository
    {
        Task<long?> GetLastIndexedBlockNumberAsync();

        Task SaveLastIndexedBlockNumber(long blockNumber);
    }
}
