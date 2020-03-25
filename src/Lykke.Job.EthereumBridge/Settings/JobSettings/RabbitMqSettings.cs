using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.EthereumBridge.Settings.JobSettings
{
    public class RabbitMqSettings
    {
        [AmqpCheck]
        public string RabbitMqConnectionString { get; set; }
    }
}
