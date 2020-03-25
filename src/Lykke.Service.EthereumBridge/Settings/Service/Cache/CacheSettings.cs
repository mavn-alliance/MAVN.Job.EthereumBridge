using System;
using JetBrains.Annotations;

namespace Lykke.Service.EthereumBridge.Settings.Service.Cache
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class CacheSettings
    {
        public TimeSpan BalanceExpirationPeriod { get; set; }
    }
}
