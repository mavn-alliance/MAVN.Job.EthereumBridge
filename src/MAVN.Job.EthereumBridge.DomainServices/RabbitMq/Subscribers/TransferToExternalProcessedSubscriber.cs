using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Subscriber;
using MAVN.Service.CrossChainTransfers.Contract;
using MAVN.Job.EthereumBridge.Domain.RabbitMq.Handlers;

namespace MAVN.Job.EthereumBridge.DomainServices.RabbitMq.Subscribers
{
    public class TransferToExternalProcessedSubscriber : JsonRabbitSubscriber<TransferToExternalProcessedEvent>
    {
        private readonly ITransferToExternalProcessedHandler _handler;
        private readonly ILog _log;

        public TransferToExternalProcessedSubscriber(
            ITransferToExternalProcessedHandler handler,
            string connectionString,
            string exchangeName,
            string queueName,
            ILogFactory logFactory)
            : base(connectionString, exchangeName, queueName, logFactory)
        {
            _handler = handler;
            _log = logFactory.CreateLog(this);
        }

        protected override async Task ProcessMessageAsync(TransferToExternalProcessedEvent message)
        {
            await _handler.HandleAsync(message.PrivateAddress, message.PublicAddress, message.InternalTransferId, 
                message.OperationId, message.Amount);
            _log.Info("Processed TransferToExternalProcessedEvent", message);
        }
    }
}
