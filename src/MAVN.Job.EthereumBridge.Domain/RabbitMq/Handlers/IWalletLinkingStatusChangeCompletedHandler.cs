using System.Threading.Tasks;

namespace MAVN.Job.EthereumBridge.Domain.RabbitMq.Handlers
{
    public interface IWalletLinkingStatusChangeCompletedHandler
    {
        Task HandleAsync(string operationId, string internalAddress, string publicAddress);
    }
}
