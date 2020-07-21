using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Job.EthereumBridge.Domain.Enums;
using MAVN.Job.EthereumBridge.Domain.Models;
using MAVN.Persistence.PostgreSQL.Legacy;

namespace MAVN.Job.EthereumBridge.Domain.Repositories
{
    public interface IOperationsRepository
    {
        Task<(string Data, string Hash)> GetOrCreateOperationAsync(string operationId);
        Task SetTransactionDataAsync(
            string operationId,
            string transactionData,
            TransactionContext txContext);
        Task SetTransactionHashAsync(string operationId, string transactionHash);
        Task<IEnumerable<HangedOperationDto>> GetHandedOperationsIds(DateTime timestampBeforeConsideredHanged);
        Task<BroadcastedOperationDto[]> GetBroadcastedOperations();
        Task SetTransactionStatusAsync(string operationId, OperationStatus status);
    }
}
