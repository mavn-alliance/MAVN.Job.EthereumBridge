using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Lykke.Job.EthereumBridge.Domain.Enums;

namespace Lykke.Job.EthereumBridge.MsSqlRepositories.Entities
{
    [Table("operations")]
    public class OperationEntity
    {
        [Key, Required]
        [Column("id")]
        public string Id { get; set; }

        [Column("transaction_hash")]
        public string TransactionHash { get; set; }

        [Column("transaction_data")]
        public string TransactionData { get; set; }

        [Required]
        [Column("last_updated")]
        public DateTime LastUpdated { get; set; }

        [Required]
        [Column("operation_status")]
        public OperationStatus OperationStatus { get; set; }
    }
}
