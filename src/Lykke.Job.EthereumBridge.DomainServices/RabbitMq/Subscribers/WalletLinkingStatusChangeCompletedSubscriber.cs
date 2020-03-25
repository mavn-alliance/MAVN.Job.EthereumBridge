using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.EthereumBridge.Domain.RabbitMq.Handlers;
using Lykke.RabbitMqBroker.Subscriber;
using Lykke.Service.CrossChainWalletLinker.Contract.Linking;

namespace Lykke.Job.EthereumBridge.DomainServices.RabbitMq.Subscribers
{
    public class WalletLinkingStatusChangeCompletedSubscriber : JsonRabbitSubscriber<WalletLinkingStatusChangeCompletedEvent>
    {
        private readonly IWalletLinkingStatusChangeCompletedHandler _handler;
        private readonly ILog _log;

        public WalletLinkingStatusChangeCompletedSubscriber(
            IWalletLinkingStatusChangeCompletedHandler handler,
            string connectionString,
            string exchangeName,
            string queueName,
            ILogFactory logFactory) : base(connectionString, exchangeName, queueName, logFactory)
        {
            _handler = handler;
            _log = logFactory.CreateLog(this);
        }

        protected override async Task ProcessMessageAsync(WalletLinkingStatusChangeCompletedEvent message)
        {
            await _handler.HandleAsync(message.EventId, message.PrivateAddress, message.PublicAddress);
            _log.Info("Processed WalletLinkingStatusChangeRequestedEvent", message);
        }
    }
}
