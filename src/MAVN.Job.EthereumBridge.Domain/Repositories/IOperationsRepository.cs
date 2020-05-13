using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MAVN.Common.MsSql;
using MAVN.Job.EthereumBridge.Domain.Enums;
using MAVN.Job.EthereumBridge.Domain.Models;

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
