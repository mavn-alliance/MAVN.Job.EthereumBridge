using System.Net;
using System.Threading.Tasks;
using Lykke.Service.EthereumBridge.Client.Api;
using Lykke.Service.EthereumBridge.Client.Models.Wallets;
using Lykke.Job.EthereumBridge.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumBridge.Controllers
{
    [ApiController]
    [Route("api/wallets")]
    public class WalletsController : ControllerBase, IWalletsApi
    {
        private readonly IBalancesService _balancesService;

        public WalletsController(IBalancesService balancesService)
        {
            _balancesService = balancesService;
        }

        /// <summary>
        /// Returns a balance by wallet address.
        /// </summary>
        /// <param name="walletAddress">The wallet address in the public blockchain.</param>
        /// <returns>The wallet balance.</returns>
        [HttpGet("{walletAddress}/balance")]
        [ProducesResponseType(typeof(BalanceModel), (int) HttpStatusCode.OK)]
        public async Task<BalanceModel> GetBalanceAsync(string walletAddress)
        {
            var balance = await _balancesService.GetAsync(walletAddress);

            return new BalanceModel {Amount = balance};
        }
    }
}
