using System;
using Falcon.Numerics;

namespace MAVN.Job.EthereumBridge.Contract
{
    public class TransferToExternalCompletedEvent
    {
        /// <summary>
        /// Unique identifier of the event
        /// </summary>
        public string EventId { get; set; }

        /// <summary>
        /// Eth wallet address of the customer
        /// </summary>
        public string PublicAddress { get; set; }

        /// <summary>
        /// Wallet address of the customer in the internal network
        /// </summary>
        public string PrivateAddress { get; set; }

        /// <summary>
        /// Amount of tokens
        /// </summary>
        public Money18 Amount { get; set; }

        /// <summary>
        /// Timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
