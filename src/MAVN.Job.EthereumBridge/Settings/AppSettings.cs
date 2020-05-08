using Lykke.Sdk.Settings;
using MAVN.Service.QuorumTransactionSigner.Client;
using MAVN.Job.EthereumBridge.Settings.JobSettings;

namespace MAVN.Job.EthereumBridge.Settings
{
    public class AppSettings : BaseAppSettings
    {
        public EthereumBridgeJobSettings EthereumBridgeJob { get; set; }

        public QuorumTransactionSignerServiceClientSettings QuorumTransactionSignerService { get; set; }
    }
}
