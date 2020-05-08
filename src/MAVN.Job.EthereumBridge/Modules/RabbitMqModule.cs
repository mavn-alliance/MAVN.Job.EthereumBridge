using Autofac;
using JetBrains.Annotations;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;
using MAVN.Service.CrossChainTransfers.Contract;
using MAVN.Service.CrossChainWalletLinker.Contract.Linking;
using Lykke.SettingsReader;
using MAVN.Job.EthereumBridge.Contract;
using MAVN.Job.EthereumBridge.DomainServices.RabbitMq.Subscribers;
using MAVN.Job.EthereumBridge.Settings;
using MAVN.Job.EthereumBridge.Settings.JobSettings;

namespace MAVN.Job.EthereumBridge.Modules
{
    [UsedImplicitly]
    public class RabbitMqModule : Module
    {
        private const string EthereumWalletLinkingStatusChangeCompletedExchangeName = "lykke.wallet.ethereumwalletlinkingstatuschangecompleted";
        private const string TransferToExternalProcessedExchangeName = "lykke.wallet.transfertoexternalprocessed";
        private const string WalletLinkingStatusChangeCompletedExchangeName = "lykke.wallet.walletlinkingstatuschangecompleted";
        private const string TransferToExternalCompletedExchangeName = "lykke.wallet.transfertoexternalcompleted";
        private const string TransferToInternalDetectedExchangeName = "lykke.wallet.transfertointernaldetected";
        private const string SeizeToInternalDetectedExchangeName = "lykke.wallet.seizetointernaldetected";

        private const string DefaultQueueName = "ethereumbridge";

        private readonly RabbitMqSettings _settings;

        public RabbitMqModule(IReloadingManager<AppSettings> appSettings)
        {
            _settings = appSettings.CurrentValue.EthereumBridgeJob.RabbitMq;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var rabbitMqConnString = _settings.RabbitMqConnectionString;

            builder.RegisterJsonRabbitSubscriber<TransferToExternalProcessedSubscriber, TransferToExternalProcessedEvent>(
                rabbitMqConnString,
                TransferToExternalProcessedExchangeName,
                DefaultQueueName);

            builder.RegisterJsonRabbitSubscriber<WalletLinkingStatusChangeCompletedSubscriber, WalletLinkingStatusChangeCompletedEvent>(
                rabbitMqConnString,
                WalletLinkingStatusChangeCompletedExchangeName,
                DefaultQueueName);

            builder.RegisterJsonRabbitPublisher<EthereumWalletLinkingStatusChangeCompletedEvent>(
                rabbitMqConnString,
                EthereumWalletLinkingStatusChangeCompletedExchangeName);

            builder.RegisterJsonRabbitPublisher<TransferToExternalCompletedEvent>(
                rabbitMqConnString,
                TransferToExternalCompletedExchangeName);

            builder.RegisterJsonRabbitPublisher<TransferToInternalDetectedEvent>(
                rabbitMqConnString,
                TransferToInternalDetectedExchangeName);

            builder.RegisterJsonRabbitPublisher<SeizeToInternalDetectedEvent>(
                rabbitMqConnString,
                SeizeToInternalDetectedExchangeName);
        }
    }
}
