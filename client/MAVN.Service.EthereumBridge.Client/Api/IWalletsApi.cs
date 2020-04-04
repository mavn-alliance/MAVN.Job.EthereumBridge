using System.Threading.Tasks;
using JetBrains.Annotations;
using MAVN.Service.EthereumBridge.Client.Models.Wallets;
using Refit;

namespace MAVN.Service.EthereumBridge.Client.Api
{
    /// <summary>
    /// The operations contract for wallets.
    /// </summary>
    [PublicAPI]
    public interface IWalletsApi
    {
        /// <summary>
        /// Returns a balance by wallet address.
        /// </summary>
        /// <param name="walletAddress">The wallet address in the public blockchain.</param>
        /// <returns>The wallet balance.</returns>
        [Get("/api/wallets/{walletAddress}/balance")]
        Task<BalanceModel> GetBalanceAsync(string walletAddress);
    }
}
