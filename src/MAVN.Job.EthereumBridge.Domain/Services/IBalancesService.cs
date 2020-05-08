using System.Threading.Tasks;
using MAVN.Numerics;

namespace MAVN.Job.EthereumBridge.Domain.Services
{
    public interface IBalancesService
    {
        Task<Money18> GetAsync(string walletAddress);
    }
}
