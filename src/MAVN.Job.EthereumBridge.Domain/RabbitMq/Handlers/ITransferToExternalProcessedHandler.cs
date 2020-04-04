using System.Numerics;
using System.Threading.Tasks;
using Falcon.Numerics;

namespace MAVN.Job.EthereumBridge.Domain.RabbitMq.Handlers
{
    public interface ITransferToExternalProcessedHandler
    {
        Task HandleAsync(
            string privateAddress,
            string publicAddress,
            BigInteger internalTransferId,
            string operationId,
            Money18 amount);
    }
}
