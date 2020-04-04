using System;
using JetBrains.Annotations;

namespace MAVN.Service.EthereumBridge.Settings.Service.Cache
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class CacheSettings
    {
        public TimeSpan BalanceExpirationPeriod { get; set; }
    }
}
