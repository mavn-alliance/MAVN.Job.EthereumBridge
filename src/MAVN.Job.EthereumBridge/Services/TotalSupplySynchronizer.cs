using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Common.Log;
using MAVN.Job.EthereumBridge.Domain.Services;
using MAVN.PublicBlockchain.Services;


namespace MAVN.Job.EthereumBridge.Services
{
    public class TotalSupplySynchronizer : TimerPeriod
    {
        private static readonly TimeSpan DefaultIdlePeriod = TimeSpan.FromHours(1);
        
        private readonly MVNTokenService _tokenServiceForPublicNetwork;
        private readonly PrivateBlockchain.Services.MVNTokenService _tokenServiceForInternalNetwork;
        private readonly IOperationsService _operationsService;
        private readonly ISettingsService _settingsService;
        private readonly IBlockchainEncodingService _blockchainEncodingService;
        private readonly ILog _log;
        
        public TotalSupplySynchronizer(
            MVNTokenService tokenServiceForPublicNetwork,
            PrivateBlockchain.Services.MVNTokenService tokenServiceForInternalNetwork,
            IOperationsService operationsService,
            ISettingsService settingsService,
            IBlockchainEncodingService blockchainEncodingService,
            TimeSpan? idlePeriod,
            ILogFactory logFactory)
            : base(idlePeriod ?? DefaultIdlePeriod, logFactory)
        {
            _tokenServiceForPublicNetwork = tokenServiceForPublicNetwork;
            _tokenServiceForInternalNetwork = tokenServiceForInternalNetwork;
            _operationsService = operationsService;
            _settingsService = settingsService;
            _blockchainEncodingService = blockchainEncodingService;
            _log = logFactory.CreateLog(this);
        }
        
        public override async Task Execute()
        {
            var totalSupplyInternal = await _tokenServiceForInternalNetwork.TotalSupplyQueryAsync();

            var internalSupplyInPublicNetwork = await _tokenServiceForPublicNetwork.TotalSupplyQueryAsync();

            if (internalSupplyInPublicNetwork != totalSupplyInternal)
            {
                _log.Info("Total supply in private blockchain differs from the one in public and will be synced",
                    new {totalSupplyInternal, internalSupplyInPublicNetwork});

                var encodedData = _blockchainEncodingService.EncodeSetInternalSupplyData(totalSupplyInternal);

                var operationId = Guid.NewGuid();

                await _operationsService.ExecuteOperationAsync(operationId.ToString(),
                    _settingsService.GetMasterWalletAddress(),
                    _settingsService.GetPublicBlockchainGatewayContractAddress(),
                    encodedData);

                _log.Info("Posted transaction to update internal supply in public", new {operationId});
            }
        }
    }
}
