using System.Data.Common;
using JetBrains.Annotations;
using Lykke.Common.MsSql;
using Lykke.Job.EthereumBridge.MsSqlRepositories.Entities;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Job.EthereumBridge.MsSqlRepositories
{
    public class EthereumBridgeContext : MsSqlContext
    {
        private const string Schema = "ethereum_bridge";

        internal DbSet<BlocksDataEntity> BlocksData { get; set; }

        internal DbSet<NonceEntity> Nonces { get; set; }

        internal DbSet<OperationEntity> Operations { get; set; }

        // C-tor for EF migrations
        [UsedImplicitly]
        public EthereumBridgeContext()
            : base(Schema)
        {
        }

        public EthereumBridgeContext(string connectionString, bool isTraceEnabled)
            : base(Schema, connectionString, isTraceEnabled)
        {
        }

        public EthereumBridgeContext(DbConnection dbConnection)
            : base(Schema, dbConnection)
        {
        }

        protected override void OnLykkeModelCreating(
            ModelBuilder modelBuilder)
        {
            var operationEntityBuilder = modelBuilder.Entity<OperationEntity>();
            operationEntityBuilder.HasIndex(x => new { x.OperationStatus, x.LastUpdated }).IsUnique(false);
        }
    }
}
