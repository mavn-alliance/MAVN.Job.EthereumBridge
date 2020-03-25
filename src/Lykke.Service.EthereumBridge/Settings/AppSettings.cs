using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using Lykke.Service.EthereumBridge.Settings.Service;

namespace Lykke.Service.EthereumBridge.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public EthereumBridgeSettings EthereumBridgeService { get; set; }
    }
}
