using Lykke.SettingsReader.Attributes;

namespace MAVN.Job.EthereumBridge.Settings.JobSettings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string RabbitMqConnectionString { get; set; }
    }
}
