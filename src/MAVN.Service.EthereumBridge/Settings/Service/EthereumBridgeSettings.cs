using JetBrains.Annotations;
using MAVN.Service.EthereumBridge.Settings.Service.Blockchain;
using MAVN.Service.EthereumBridge.Settings.Service.Cache;
using MAVN.Service.EthereumBridge.Settings.Service.Db;
using MAVN.Service.EthereumBridge.Settings.Service.Redis;

namespace MAVN.Service.EthereumBridge.Settings.Service
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class EthereumBridgeSettings
    {
        public DbSettings Db { get; set; }

        public BlockchainSettings Blockchain { get; set; }

        public CacheSettings Cache { get; set; }

        public RedisSettings Redis { get; set; }
    }
}
