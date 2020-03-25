using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Common.MsSql;
using Lykke.Job.EthereumBridge.Domain.Enums;
using Lykke.Job.EthereumBridge.Domain.Models;
using Lykke.Job.EthereumBridge.Domain.Repositories;
using Lykke.Job.EthereumBridge.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Job.EthereumBridge.MsSqlRepositories.Repositories
{
    public class OperationsRepository : IOperationsRepository
    {
        private readonly IDbContextFactory<EthereumBridgeContext> _contextFactory;

        public OperationsRepository(IDbContextFactory<EthereumBridgeContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<(string Data, string Hash)> GetOrCreateOperationAsync(string operationId)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var operation = await context.Operations.FindAsync(operationId);

                if (operation == null)
                {
                    operation = new OperationEntity
                    {
                        Id = operationId,
                        LastUpdated = DateTime.UtcNow,
                        OperationStatus = OperationStatus.Created
                    };

                    context.Operations.Add(operation);

                    await context.SaveChangesAsync();
                }

                return (operation.TransactionData, operation.TransactionHash);
            }
        }

        public async Task SetTransactionDataAsync(
            string operationId,
            string transactionData,
            TransactionContext txContext)
        {
            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                var operation = await context.Operations.FindAsync(operationId);

                if (operation == null)
                    throw new InvalidOperationException($"Operation with id {operationId} does not exist");

                if (string.IsNullOrEmpty(operation.TransactionData))
                {
                    operation.TransactionData = transactionData;
                    operation.LastUpdated = DateTime.UtcNow;
                    operation.OperationStatus = OperationStatus.Built;

                    context.Operations.Update(operation);

                    await context.SaveChangesAsync();
                }
            }
        }

        public async Task SetTransactionHashAsync(string operationId, string transactionHash)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var operation = await context.Operations.FindAsync(operationId);

                if (operation == null)
                    throw new InvalidOperationException($"Operation with id {operationId} does not exist");

                if (string.IsNullOrEmpty(operation.TransactionHash))
                {
                    operation.TransactionHash = transactionHash;
                    operation.LastUpdated = DateTime.UtcNow;
                    operation.OperationStatus = OperationStatus.Broadcasted;

                    context.Operations.Update(operation);

                    await context.SaveChangesAsync();

                    return;
                }

                if (operation.TransactionHash != transactionHash)
                    throw new InvalidOperationException(
                        $"An attempt to update transaction with different hash (id={operationId}, transactionHash={operation.TransactionHash}, newHash={transactionHash})");
            }
        }

        public async Task SetTransactionStatusAsync(string operationId, OperationStatus status)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var operation = await context.Operations.FindAsync(operationId);

                if (operation == null)
                    throw new InvalidOperationException($"Operation with id {operationId} does not exist");

                operation.OperationStatus = status;
                operation.LastUpdated = DateTime.UtcNow;

                context.Operations.Update(operation);

                await context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<HangedOperationDto>> GetHandedOperationsIds(DateTime timestampBeforeConsideredHanged)
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.Operations
                    .Where(o => (o.OperationStatus == OperationStatus.Broadcasted || o.OperationStatus == OperationStatus.Built) &&
                                o.LastUpdated < timestampBeforeConsideredHanged)
                    .Select(o => new HangedOperationDto { OperationId = o.Id, TransactionData = o.TransactionData })
                    .ToArrayAsync();

                return result;
            }
        }

        public async Task<BroadcastedOperationDto[]> GetBroadcastedOperations()
        {
            using (var context = _contextFactory.CreateDataContext())
            {
                var result = await context.Operations
                    .Where(o => o.OperationStatus == OperationStatus.Broadcasted)
                    .Select(o => new BroadcastedOperationDto { OperationId = o.Id, TransactionHash = o.TransactionHash })
                    .ToArrayAsync();

                return result;
            }
        }
    }
}
