using System.Data.Common;
using JetBrains.Annotations;
using MAVN.Job.EthereumBridge.MsSqlRepositories.Entities;
using MAVN.Persistence.PostgreSQL.Legacy;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Job.EthereumBridge.MsSqlRepositories
{
    public class EthereumBridgeContext : PostgreSQLContext
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

        protected override void OnMAVNModelCreating(
            ModelBuilder modelBuilder)
        {
            var operationEntityBuilder = modelBuilder.Entity<OperationEntity>();
            operationEntityBuilder.HasIndex(x => new { x.OperationStatus, x.LastUpdated }).IsUnique(false);
        }
    }
}
