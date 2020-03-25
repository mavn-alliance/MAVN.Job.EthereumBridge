using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.EthereumBridge.Domain.Enums;
using Lykke.Job.EthereumBridge.Domain.Repositories;
using Lykke.Job.EthereumBridge.Domain.Services;

namespace Lykke.Job.EthereumBridge.DomainServices.Services
{
    public class TransactionWatcherService : ITransactionWatcherService
    {
        private readonly IEthereumNodeJobClient _ethereumNodeJobClient;
        private readonly IOperationsRepository _operationsRepository;
        private readonly ILog _log;

        public TransactionWatcherService(
            IEthereumNodeJobClient ethereumNodeJobClient,
            IOperationsRepository operationsRepository,
            ILogFactory logFactory)
        {
            _ethereumNodeJobClient = ethereumNodeJobClient;
            _operationsRepository = operationsRepository;
            _log = logFactory.CreateLog(this);
        }

        public async Task ProcessTransactions()
        {
            var broadcastedOperations = await _operationsRepository.GetBroadcastedOperations();

            foreach (var operation in broadcastedOperations)
            {
                var status = await _ethereumNodeJobClient.GetTransactionStatus(operation.TransactionHash);

                if (!status.HasValue)
                    continue;

                var operationStatus = status.Value == 0 ? OperationStatus.Failed : OperationStatus.Succeeded;
                await _operationsRepository.SetTransactionStatusAsync(operation.OperationId, operationStatus);

                _log.Info($"Status updated to {status} for operation with id {operation.OperationId}");
            }
        }
    }
}
