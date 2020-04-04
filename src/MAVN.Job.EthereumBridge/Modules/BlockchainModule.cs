using Autofac;
using Lykke.Common.Log;
using Lykke.Logs.Nethereum;
using Lykke.SettingsReader;
using MAVN.Job.EthereumBridge.Domain.Services;
using MAVN.Job.EthereumBridge.DomainServices.Services;
using MAVN.Job.EthereumBridge.Settings;
using MAVN.Job.EthereumBridge.Settings.JobSettings;
using Nethereum.Contracts.Services;
using Nethereum.JsonRpc.WebSocketClient;
using Nethereum.RPC.Eth.Services;
using Nethereum.Web3;

namespace MAVN.Job.EthereumBridge.Modules
{
    public class BlockchainModule : Module
    {
        private const string Web3ForPublicName = "Web3ForPublicBlockchain";
        private const string Web3ForInternalName = "Web3ForInternalBlockchain";

        private readonly BlockchainSettings _blockchainSettings;

        public BlockchainModule(IReloadingManager<AppSettings> appSettings)
        {
            _blockchainSettings = appSettings.CurrentValue.EthereumBridgeJob.Blockchain;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => RegisterWeb3(x, _blockchainSettings.InternalTransactionNodeUrl))
                .Named<Web3>(Web3ForInternalName)
                .SingleInstance();

            builder
                .Register(ctx =>
                    new PrivateBlockchain.Services.MVNTokenService(ctx.ResolveNamed<Web3>(Web3ForInternalName),
                        _blockchainSettings.PrivateBlockchainTokenContractAddress))
                .AsSelf()
                .SingleInstance();

            builder.Register(x => RegisterWeb3(x, _blockchainSettings.PublicTransactionNodeUrl))
                .Named<Web3>(Web3ForPublicName)
                .SingleInstance();

            builder
                .Register(ctx => ctx.ResolveNamed<Web3>(Web3ForPublicName).Eth.Transactions)
                .As<IEthApiTransactionsService>()
                .SingleInstance();

            builder
                .Register(ctx => ctx.ResolveNamed<Web3>(Web3ForPublicName).Eth)
                .As<IEthApiContractService>()
                .SingleInstance();

            builder
                .Register(ctx =>
                    new PublicBlockchain.Services.MVNTokenService(ctx.ResolveNamed<Web3>(Web3ForPublicName),
                        _blockchainSettings.PublicBlockchainTokenContractAddress))
                .AsSelf()
                .SingleInstance();

            builder
                .RegisterType<EthereumNodeJobClient>()
                .As<IEthereumNodeJobClient>()
                .SingleInstance();

            builder.RegisterType<BlockchainIndexingService>()
                .As<IBlockchainIndexingService>()
                .WithParameter("blockBatchSize", _blockchainSettings.BlockBatchSize)
                .WithParameter("blockConfirmationLevel", _blockchainSettings.BlockConfirmationLevel)
                .SingleInstance();

            builder.RegisterType<BlockchainEncodingService>()
                .As<IBlockchainEncodingService>()
                .SingleInstance();
        }

        private Web3 RegisterWeb3(IComponentContext ctx, string nodeUrl)
        {
            var logFactory = ctx.Resolve<ILogFactory>();

            if (nodeUrl.StartsWith("ws"))
            {
                var wsClient = new WebSocketClient(nodeUrl, log: logFactory.CreateNethereumLog(this));
                return new Web3(wsClient);
            }

            return new Web3(nodeUrl, logFactory.CreateNethereumLog(this));
        }
    }
}
