using Falcon.Numerics;
using JetBrains.Annotations;

namespace Lykke.Service.EthereumBridge.Client.Models.Wallets
{
    /// <summary>
    /// Represents a wallet balance. 
    /// </summary>
    [PublicAPI]
    public class BalanceModel
    {
        /// <summary>
        /// The available balance.
        /// </summary>
        public Money18 Amount { get; set; }
    }
}
