using System;
using System.Threading.Tasks;
using MAVN.Numerics;
using MAVN.Job.EthereumBridge.Domain.Services;
using Microsoft.Extensions.Caching.Distributed;
using MAVN.PublicBlockchain.Services;
using Nethereum.RPC.Eth.DTOs;

namespace MAVN.Job.EthereumBridge.DomainServices.Services
{
    public class BalancesService : IBalancesService
    {
        private readonly MVNTokenService _tokenService;
        private readonly IDistributedCache _distributedCache;
        private readonly TimeSpan _cacheExpirationPeriod;
        private readonly IEthereumNodeServiceClient _ethereumNodeServiceClient;

        public BalancesService(
            MVNTokenService tokenService,
            IDistributedCache distributedCache,
            TimeSpan cacheExpirationPeriod, 
            IEthereumNodeServiceClient ethereumNodeServiceClient)
        {
            _tokenService = tokenService;
            _distributedCache = distributedCache;
            _cacheExpirationPeriod = cacheExpirationPeriod;
            _ethereumNodeServiceClient = ethereumNodeServiceClient;
        }

        public async Task<Money18> GetAsync(string walletAddress)
        {
            var cachedBalance = await GetCachedValue(walletAddress);

            if (cachedBalance != null)
                return cachedBalance.Value;

            var blockNumber = await _ethereumNodeServiceClient.GetBlockNumberAsync();

            var value = await _tokenService.BalanceOfQueryAsync(walletAddress,
                new BlockParameter((ulong) blockNumber));

            var balance = Money18.CreateFromAtto(value);

            await SetCacheValueAsync(walletAddress, balance);

            return balance;
        }

        private async Task SetCacheValueAsync(string walletAddress, Money18 value)
        {
            await _distributedCache.SetStringAsync(GetCacheKey(walletAddress),
                value.ToString(),
                new DistributedCacheEntryOptions {AbsoluteExpiration = DateTime.UtcNow.Add(_cacheExpirationPeriod)});
        }

        private async Task<Money18?> GetCachedValue(string walletAddress)
        {
            var value = await _distributedCache.GetStringAsync(GetCacheKey(walletAddress));

            return string.IsNullOrEmpty(value)
                ? new Money18?()
                : Money18.Parse(value);
        }

        private static string GetCacheKey(string walletAddress)
            => $"ethereum-bridge:wallet-balance:{walletAddress}";
    }
}
