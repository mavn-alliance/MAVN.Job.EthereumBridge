using System.Threading.Tasks;
using Falcon.Numerics;

namespace Lykke.Job.EthereumBridge.Domain.Services
{
    public interface IBalancesService
    {
        Task<Money18> GetAsync(string walletAddress);
    }
}
