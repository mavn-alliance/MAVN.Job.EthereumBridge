using Lykke.SettingsReader.Attributes;

namespace MAVN.Job.EthereumBridge.Settings.JobSettings
{
    public class BlockchainSettings
    {
        public string PublicBlockchainGatewayContractAddress { get; set; }

        public string PrivateBlockchainGatewayContractAddress { get; set; }

        public string PublicBlockchainTokenContractAddress { get; set; }

        public string PrivateBlockchainTokenContractAddress { get; set; }

        public string MasterWalletAddress { get; set; }

        public string PublicTransactionNodeUrl { get; set; }

        public string InternalTransactionNodeUrl { get; set; }

        public int BlockConfirmationLevel { get; set; }

        [Optional]
        public int? BlockBatchSize { get; set; }

        public long GasLimit { get; set; }

        public long GasPrice { get; set; }
        
        public long MasterWalletBalanceWarningLevelInWei { get; set; }
    }
}
