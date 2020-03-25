using JetBrains.Annotations;

namespace Lykke.Service.EthereumBridge.Settings.Service.Redis
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RedisSettings
    {
        public string InstanceName { get; set; }

        public string Configuration { get; set; }
    }
}
