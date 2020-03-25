using System.Threading;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.Services;
using Nethereum.Util;

namespace Lykke.Job.EthereumBridge.DomainServices.Extensions
{
    internal static class EthApiTransactionsServiceExtensions
    {
        public static async Task<string> SendTransactionAsync(
            this IEthApiTransactionsService service,
            string signedTransactionData,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var transactionHash = Sha3Keccack.Current.CalculateHashFromHex(signedTransactionData);
            var transactionHashWithPrefix = $"0x{transactionHash}";

            var transaction = await service.GetTransactionByHash.SendRequestAsync(transactionHashWithPrefix);

            // If transaction is not null, the transaction has already been broadcasted
            // and it is not necessary to broadcast it again.

            if (transaction != null)
            {
                return transactionHashWithPrefix;
            }

            // Broadcasting transaction

            try
            {
                await service.SendRawTransaction.SendRequestAsync(signedTransactionData);
            }
            // Unfortunately, there are neither specific error code, nor specific properties for this case in an exception  
            catch (RpcResponseException e) when (e.RpcError.Message == $"known transaction: {transactionHash}")
            {
                return transactionHashWithPrefix;
            }

            // ...and waiting, until it appears on a blockchain node.

            transaction = await service.GetTransactionByHash.SendRequestAsync(transactionHashWithPrefix);

            while (transaction == null)
            {
                await Task.Delay(100, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                transaction = await service.GetTransactionByHash.SendRequestAsync(transactionHashWithPrefix);
            }

            return transactionHashWithPrefix;
        }
    }
}
