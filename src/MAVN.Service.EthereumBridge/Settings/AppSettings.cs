using JetBrains.Annotations;
using Lykke.Sdk.Settings;
using MAVN.Service.EthereumBridge.Settings.Service;

namespace MAVN.Service.EthereumBridge.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public EthereumBridgeSettings EthereumBridgeService { get; set; }
    }
}
