using JetBrains.Annotations;
using MAVN.Service.EthereumBridge.Client.Api;

namespace MAVN.Service.EthereumBridge.Client
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
