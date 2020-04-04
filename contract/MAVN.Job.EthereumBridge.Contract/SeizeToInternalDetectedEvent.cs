using System;
using Falcon.Numerics;

namespace MAVN.Job.EthereumBridge.Contract
{
    /// <summary>
    /// Represents the seize event in the public network.
    /// </summary>
    public class SeizeToInternalDetectedEvent
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        public string OperationId { get; set; }

        /// <summary>
        /// The public account.
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// The reason to seize.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// The amount of tokens.
        /// </summary>
        public Money18 Amount { get; set; }

        /// <summary>
        /// The date and time of event.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}
