using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MAVN.Job.EthereumBridge.MsSqlRepositories.Entities
{
    [Table("nonces")]
    public class NonceEntity
    {
        [Key]
        [Column("master_wallet_address")]
        public string MasterWalletAddress { get; set; }

        [Required]
        [Column("nonce")]
        public long Nonce { get; set; }

        public static NonceEntity Create(string masterWalletAddress, long nonce)
        {
            return new NonceEntity
            {
                MasterWalletAddress = masterWalletAddress,
                Nonce = nonce
            };
        }
    }
}
