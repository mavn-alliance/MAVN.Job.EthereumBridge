using JetBrains.Annotations;
using Lykke.Service.EthereumBridge.Client.Api;

namespace Lykke.Service.EthereumBridge.Client
{
    /// <summary>
    /// The Ethereum bridge API job client.
    /// </summary>
    [PublicAPI]
    public interface IEthereumBridgeClient
    {
        /// <summary>
        /// Wallets API.
        /// </summary>
        IWalletsApi Wallets { get; set; }
    }
}
