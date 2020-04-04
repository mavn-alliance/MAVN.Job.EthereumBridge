using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;

namespace MAVN.Job.EthereumBridge.Services
{
    public class TotalSupplyInconsistencyChecker : TimerPeriod
    {
        private static readonly TimeSpan IdlePeriod = TimeSpan.FromMinutes(15);

        private readonly MVNTokenService _tokenServiceForPublicNetwork;
        private readonly PrivateBlockchain.Services.MVNTokenService _tokenServiceForInternalNetwork;
        private readonly string _internalBlockchainGatewayAddress;
        private readonly ILog _log;

        public TotalSupplyInconsistencyChecker(
            MVNTokenService tokenServiceForPublicNetwork,
            PrivateBlockchain.Services.MVNTokenService tokenServiceForInternalNetwork,
            string internalBlockchainGatewayAddress,
            ILogFactory logFactory)
            : base(IdlePeriod, logFactory)
        {
            _tokenServiceForPublicNetwork = tokenServiceForPublicNetwork;
            _tokenServiceForInternalNetwork = tokenServiceForInternalNetwork;
            _internalBlockchainGatewayAddress = internalBlockchainGatewayAddress;
            _log = logFactory.CreateLog(this);
        }

        public override async Task Execute()
        {
            var tokenGatewaySupply = await _tokenServiceForInternalNetwork.BalanceOfQueryAsync(_internalBlockchainGatewayAddress);
            var publicSupply = await _tokenServiceForPublicNetwork.PublicSupplyQueryAsync();

            if (publicSupply != tokenGatewaySupply)
            {
                _log.Error(message: "Public supply is different than token gateway supply",
                    context: new { publicSupply, tokenGatewaySupply });
            }
        }
    }
}
