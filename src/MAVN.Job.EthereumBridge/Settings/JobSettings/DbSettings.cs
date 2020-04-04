using Lykke.SettingsReader.Attributes;

namespace MAVN.Job.EthereumBridge.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }

        public string DataConnString { get; set; }
    }
}
