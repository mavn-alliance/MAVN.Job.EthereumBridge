using Lykke.Job.EthereumBridge.Settings.JobSettings;
using Lykke.Sdk.Settings;
using Lykke.Service.QuorumTransactionSigner.Client;

namespace Lykke.Job.EthereumBridge.Settings
{
    public class AppSettings : BaseAppSettings
    {
        public EthereumBridgeJobSettings EthereumBridgeJob { get; set; }

        public QuorumTransactionSignerServiceClientSettings QuorumTransactionSignerService { get; set; }
    }
}
