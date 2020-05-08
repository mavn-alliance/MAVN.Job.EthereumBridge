using Autofac;
using JetBrains.Annotations;
using Lykke.Common;
using Lykke.Sdk;
using Lykke.Sdk.Health;
using MAVN.Service.QuorumTransactionSigner.Client;
using Lykke.SettingsReader;
using MAVN.Job.EthereumBridge.Domain.RabbitMq.Handlers;
using MAVN.Job.EthereumBridge.Domain.Services;
using MAVN.Job.EthereumBridge.DomainServices.RabbitMq.Handlers;
using MAVN.Job.EthereumBridge.DomainServices.Services;
using MAVN.Job.EthereumBridge.Services;
using MAVN.Job.EthereumBridge.Settings;

namespace MAVN.Job.EthereumBridge.Modules
{
    [UsedImplicitly]
    public class JobModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;

        public JobModule(IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>()
                .SingleInstance();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>()
                .AutoActivate()
                .SingleInstance();

            builder.RegisterType<IndexingService>()
                .As<IStartStop>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<HangedOperationsService>()
                .As<IStartStop>()
                .AsSelf()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.EthereumBridgeJob
                    .HangedOperationsJobIdlePeriod))
                .SingleInstance();

            builder.RegisterType<TransactionsStatusChecker>()
                .As<IStartStop>()
                .AsSelf()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.EthereumBridgeJob
                    .BroadcastedTransactionsStatusCheckJobIdlePeriod))
                .SingleInstance();

            builder.RegisterType<MasterWalletBalanceChecker>()
                .As<IStartStop>()
                .AsSelf()
                .WithParameter("masterWalletAddress",
                    _appSettings.CurrentValue.EthereumBridgeJob.Blockchain.MasterWalletAddress)
                .WithParameter("balanceWarningLevel",
                    _appSettings.CurrentValue.EthereumBridgeJob.Blockchain.MasterWalletBalanceWarningLevelInWei)
                .SingleInstance();

            builder.RegisterType<WalletLinkingStatusChangeCompletedHandler>()
                .As<IWalletLinkingStatusChangeCompletedHandler>()
                .SingleInstance();

            builder.RegisterType<TransferToExternalProcessedHandler>()
                .As<ITransferToExternalProcessedHandler>()
                .SingleInstance();

            builder.RegisterType<TransactionWatcherService>()
                .As<ITransactionWatcherService>()
                .SingleInstance();

            builder.RegisterType<SettingsService>()
                .As<ISettingsService>()
                .WithParameter("publicBlockchainGatewayContractAddress",
                    _appSettings.CurrentValue.EthereumBridgeJob.Blockchain.PublicBlockchainGatewayContractAddress)
                .WithParameter("publicBlockchainTokenContractAddress",
                    _appSettings.CurrentValue.EthereumBridgeJob.Blockchain.PublicBlockchainTokenContractAddress)
                .WithParameter("masterWalletAddress",
                    _appSettings.CurrentValue.EthereumBridgeJob.Blockchain.MasterWalletAddress)
                .SingleInstance();

            builder.RegisterType<OperationsService>()
                .As<IOperationsService>()
                .WithParameter("gasLimit",
                    _appSettings.CurrentValue.EthereumBridgeJob.Blockchain.GasLimit)
                .WithParameter("gasPrice",
                    _appSettings.CurrentValue.EthereumBridgeJob.Blockchain.GasPrice)
                .SingleInstance();

            builder.RegisterQuorumTransactionSignerClient(_appSettings.CurrentValue.QuorumTransactionSignerService,
                null);

            builder.RegisterType<TotalSupplyInconsistencyChecker>()
                .WithParameter(TypedParameter.From(_appSettings.CurrentValue.EthereumBridgeJob.Blockchain.PrivateBlockchainGatewayContractAddress))
                .As<IStartStop>()
                .AsSelf()
                .SingleInstance();

            builder.RegisterType<TotalSupplySynchronizer>()
                .WithParameter("idlePeriod", _appSettings.CurrentValue.EthereumBridgeJob.TotalSupplySyncPeriod)
                .As<IStartStop>()
                .AsSelf()
                .SingleInstance();
        }
    }
}
