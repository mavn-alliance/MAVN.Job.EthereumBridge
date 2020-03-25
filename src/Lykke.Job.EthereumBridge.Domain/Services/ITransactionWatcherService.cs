using System.Threading.Tasks;

namespace Lykke.Job.EthereumBridge.Domain.Services
{
    public interface ITransactionWatcherService
    {
        Task ProcessTransactions();
    }
}
