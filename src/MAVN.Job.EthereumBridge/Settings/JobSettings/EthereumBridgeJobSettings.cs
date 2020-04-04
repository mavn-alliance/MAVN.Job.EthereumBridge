using System;
using Lykke.SettingsReader.Attributes;

namespace MAVN.Job.EthereumBridge.Settings.JobSettings
{
    public class EthereumBridgeJobSettings
    {
        public DbSettings Db { get; set; }

        public RabbitMqSettings RabbitMq { get; set; }

        public BlockchainSettings Blockchain { get; set; }

        public TimeSpan HangedOperationsJobIdlePeriod { get; set; }
        
        [Optional]
        public TimeSpan? TotalSupplySyncPeriod { get; set; }

        public TimeSpan BroadcastedTransactionsStatusCheckJobIdlePeriod { get; set; }
    }
}
