using System.Threading.Tasks;
using Lykke.Common.MsSql;

namespace MAVN.Job.EthereumBridge.Domain.Repositories
{
    public interface INoncesRepository
    {
        Task<long> GetNextNonceAsync(string masterWalletAddress, TransactionContext txContext);
    }
}
