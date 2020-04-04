using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace MAVN.Service.EthereumBridge.Client
{
    /// <summary>
    /// Represents an Ethereum bridge client settings.
    /// </summary>
    [PublicAPI]
    public class EthereumBridgeServiceClientSettings
    {
        /// <summary>
        /// The service url.
        /// </summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl { get; set; }
    }
}
