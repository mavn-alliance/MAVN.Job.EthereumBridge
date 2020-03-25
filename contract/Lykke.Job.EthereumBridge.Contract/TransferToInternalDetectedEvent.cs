using System;
using System.Numerics;
using Falcon.Numerics;

namespace Lykke.Job.EthereumBridge.Contract
{
    public class TransferToInternalDetectedEvent
    {
        /// <summary>
        /// Unique Id of the operation
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        /// Private wallet address
        /// </summary>
        public string PrivateAddress { get; set; }

        /// <summary>
        /// Public wallet address
        /// </summary>
        public string PublicAddress { get; set; }

        /// <summary>
        /// Amount of tokens
        /// </summary>
        public Money18 Amount { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Id of the transaction in the public blockchain
        /// </summary>
        public BigInteger PublicTransferId { get; set; }
    }
}
