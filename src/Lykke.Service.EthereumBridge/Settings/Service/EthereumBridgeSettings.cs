using JetBrains.Annotations;
using Lykke.Service.EthereumBridge.Settings.Service.Blockchain;
using Lykke.Service.EthereumBridge.Settings.Service.Cache;
using Lykke.Service.EthereumBridge.Settings.Service.Db;
using Lykke.Service.EthereumBridge.Settings.Service.Redis;

namespace Lykke.Service.EthereumBridge.Settings.Service
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
