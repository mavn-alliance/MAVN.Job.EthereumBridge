using System.Threading.Tasks;
using Lykke.Common.MsSql;

namespace Lykke.Job.EthereumBridge.Domain.Repositories
{
    public interface INoncesRepository
    {
        Task<long> GetNextNonceAsync(string masterWalletAddress, TransactionContext txContext);
    }
}
