using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Job.EthereumBridge.Domain.Enums;
using Lykke.Job.EthereumBridge.Domain.Models;

namespace Lykke.Job.EthereumBridge.Domain.Repositories
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
