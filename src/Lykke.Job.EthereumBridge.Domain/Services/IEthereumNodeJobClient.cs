using System.Numerics;
using System.Threading.Tasks;
using Nethereum.RPC.Eth.DTOs;

namespace Lykke.Job.EthereumBridge.Domain.Services
{
    public interface IEthereumNodeJobClient
    {
        Task<long> GetBlockNumberAsync();

        Task<FilterLog[]> GetTransactionsLogs(BigInteger fromBlock, BigInteger toBlock);

        Task<int?> GetTransactionStatus(string transactionHash);

        Task<long> GetWalletBalanceAsync(string address);
    }
}
