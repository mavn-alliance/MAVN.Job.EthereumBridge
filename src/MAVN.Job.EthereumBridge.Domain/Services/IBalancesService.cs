using System.Threading.Tasks;
using Falcon.Numerics;

namespace MAVN.Job.EthereumBridge.Domain.Services
{
    public interface IBalancesService
    {
        Task<Money18> GetAsync(string walletAddress);
    }
}
