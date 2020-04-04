using Autofac;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Logs.Nethereum;
using Lykke.SettingsReader;
using MAVN.Job.EthereumBridge.Domain.Services;
using MAVN.Job.EthereumBridge.DomainServices.Services;
using MAVN.Service.EthereumBridge.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Nethereum.Contracts.Services;
using Nethereum.JsonRpc.WebSocketClient;
using Nethereum.Web3;

namespace MAVN.Service.EthereumBridge
{
    [UsedImplicitly]
    public class AutofacModule : Module
    {
        private readonly AppSettings _settings;

        public AutofacModule(IReloadingManager<AppSettings> appSettings)
        {
            _settings = appSettings.CurrentValue;
        }

        protected override void Load(ContainerBuilder builder)
        {
            RegisterDomainServices(builder);
            RegisterBlockchainServices(builder);
            RegisterRedisCache(builder);
        }

        private void RegisterDomainServices(ContainerBuilder builder)
        {
            builder.RegisterType<BalancesService>()
                .As<IBalancesService>()
                .WithParameter("cacheExpirationPeriod",
                    _settings.EthereumBridgeService.Cache.BalanceExpirationPeriod)
                .SingleInstance();
        }

        private void RegisterBlockchainServices(ContainerBuilder builder)
        {
            builder.Register(ctx => CreateBlockchainClient(ctx.Resolve<ILogFactory>(),
                    _settings.EthereumBridgeService.Blockchain.PublicTransactionNodeUrl))
                .AsSelf()
                .SingleInstance();

            builder.Register(ctx => new MVNTokenService(ctx.Resolve<Web3>(),
                    _settings.EthereumBridgeService.Blockchain.PublicBlockchainTokenContractAddress))
                .AsSelf()
                .SingleInstance();

            builder
                .Register(ctx => ctx.Resolve<Web3>().Eth)
                .As<IEthApiContractService>()
                .SingleInstance();

            builder
                .RegisterType<EthereumNodeServiceClient>()
                .As<IEthereumNodeServiceClient>()
                .WithParameter("blockConfirmationLevel", _settings.EthereumBridgeService.Blockchain.BlockConfirmationLevel)
                .SingleInstance();
        }

        private void RegisterRedisCache(ContainerBuilder builder)
        {
            var distributedCache = new RedisCache(new RedisCacheOptions
            {
                Configuration = _settings.EthereumBridgeService.Redis.Configuration,
                InstanceName = _settings.EthereumBridgeService.Redis.InstanceName
            });

            builder.RegisterInstance(distributedCache)
                .As<IDistributedCache>()
                .SingleInstance();
        }

        private Web3 CreateBlockchainClient(ILogFactory logFactory,  string nodeUrl)
        {
            if (nodeUrl.StartsWith("ws"))
            {
                var wsClient = new WebSocketClient(nodeUrl, log: logFactory.CreateNethereumLog(this));
                return new Web3(wsClient);
            }

            return new Web3(nodeUrl, logFactory.CreateNethereumLog(this));
        }
    }
}
