using JetBrains.Annotations;

namespace MAVN.Service.EthereumBridge.Settings.Service.Blockchain
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class BlockchainSettings
    {
        public string PublicTransactionNodeUrl { get; set; }

        public string PublicBlockchainTokenContractAddress { get; set; }
        
        public int BlockConfirmationLevel { get; set; }
    }
}
