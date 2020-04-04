using System;
using System.Threading.Tasks;
using Common;
using Lykke.Common.Log;
using MAVN.Job.EthereumBridge.Domain.Services;

namespace MAVN.Job.EthereumBridge.Services
{
    public class TransactionsStatusChecker : TimerPeriod
    {
        private readonly ITransactionWatcherService _transactionWatcherService;

        public TransactionsStatusChecker(
            ITransactionWatcherService transactionWatcherService,
            ILogFactory logFactory,
            TimeSpan idleTime)
            : base(idleTime, logFactory)
        {
            _transactionWatcherService = transactionWatcherService;
        }

        public override Task Execute()
        {
            return _transactionWatcherService.ProcessTransactions();
        }
    }
}
