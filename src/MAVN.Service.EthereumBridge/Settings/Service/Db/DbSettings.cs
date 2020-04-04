using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.EthereumBridge.Settings.Service.Db
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnectionString { get; set; }
    }
}
