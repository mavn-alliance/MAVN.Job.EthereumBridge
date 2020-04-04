using JetBrains.Annotations;

namespace MAVN.Service.EthereumBridge.Settings.Service.Redis
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class RedisSettings
    {
        public string InstanceName { get; set; }

        public string Configuration { get; set; }
    }
}
