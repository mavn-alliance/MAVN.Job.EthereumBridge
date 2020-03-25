using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.EthereumBridge.Domain.Services;

namespace Lykke.Job.EthereumBridge.Services
{
    public class MasterWalletBalanceChecker : TimerPeriod
    {
        private static readonly TimeSpan IdlePeriod = TimeSpan.FromHours(1);

        private readonly string _masterWalletAddress;
        private readonly IEthereumNodeJobClient _ethereumNodeJobClient;
        private readonly long _balanceWarningLevel;
        private readonly ILog _log;
        
        public MasterWalletBalanceChecker(
            string masterWalletAddress,
            IEthereumNodeJobClient ethereumNodeJobClient, 
            long balanceWarningLevel,
            ILogFactory logFactory)
            : base(IdlePeriod, logFactory)
        {
            _masterWalletAddress = masterWalletAddress;
            _ethereumNodeJobClient = ethereumNodeJobClient;
            _balanceWarningLevel = balanceWarningLevel;
            _log = logFactory.CreateLog(this);
        }
        
        public override async Task Execute()
        {
            var balanceInWei = await _ethereumNodeJobClient.GetWalletBalanceAsync(_masterWalletAddress);

            if (balanceInWei <= _balanceWarningLevel)
            {
                _log.Warning("Almost run out of ETH on master wallet. Please deposit.",
                    context: new {walletAddress = _masterWalletAddress, balanceInWei});
            }
        }
    }
}
