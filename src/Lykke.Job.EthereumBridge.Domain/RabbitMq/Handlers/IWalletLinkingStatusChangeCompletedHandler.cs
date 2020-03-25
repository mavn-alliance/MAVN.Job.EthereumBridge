using System.Threading.Tasks;

namespace Lykke.Job.EthereumBridge.Domain.RabbitMq.Handlers
{
    public interface IWalletLinkingStatusChangeCompletedHandler
    {
        Task HandleAsync(string operationId, string internalAddress, string publicAddress);
    }
}
