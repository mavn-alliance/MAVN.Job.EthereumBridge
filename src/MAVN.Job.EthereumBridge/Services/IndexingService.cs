using System;
using System.Threading.Tasks;
using Common;
using Lykke.Common.Log;
using MAVN.Job.EthereumBridge.Domain.Services;

namespace MAVN.Job.EthereumBridge.Services
{
    public class IndexingService : TimerPeriod
    {
        private readonly IBlockchainIndexingService _blockchainIndexingService;

        public IndexingService(
            IBlockchainIndexingService blockchainIndexingService,
            ILogFactory logFactory,
            TimeSpan? idleTime = null)
            : base(idleTime ?? TimeSpan.FromSeconds(5), logFactory)
        {
            _blockchainIndexingService = blockchainIndexingService;
        }

        public override Task Execute()
        {
            return _blockchainIndexingService.IndexUntilLastBlockAsync();
        }
    }
}
