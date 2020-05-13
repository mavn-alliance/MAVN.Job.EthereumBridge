using System.Threading.Tasks;
using MAVN.Common.MsSql;

namespace MAVN.Job.EthereumBridge.Domain.Repositories
{
    public interface INoncesRepository
    {
        Task<long> GetNextNonceAsync(string masterWalletAddress, TransactionContext txContext);
    }
}
