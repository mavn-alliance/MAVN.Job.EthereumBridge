using Lykke.HttpClientGenerator;
using Lykke.Service.EthereumBridge.Client.Api;

namespace Lykke.Service.EthereumBridge.Client
{
    /// <inheritdoc /> 
    public class EthereumBridgeClient : IEthereumBridgeClient
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EthereumBridgeClient"/> with <param name="httpClientGenerator"></param>.
        /// </summary> 
        public EthereumBridgeClient(IHttpClientGenerator httpClientGenerator)
        {
            Wallets = httpClientGenerator.Generate<IWalletsApi>();
        }

        /// <inheritdoc /> 
        public IWalletsApi Wallets { get; set; }
    }
}
