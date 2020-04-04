using System.Threading.Tasks;

namespace MAVN.Job.EthereumBridge.Domain.Services
{
    public interface IOperationsService
    {
        Task RepostHangedOperations();

        Task ExecuteOperationAsync(
            string operationId,
            string masterWalletAddress,
            string targetWalletAddress,
            string encodedData);
    }
}
