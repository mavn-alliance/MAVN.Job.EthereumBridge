using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.EthereumBridge.Domain.RabbitMq.Handlers;
using Lykke.Job.EthereumBridge.Domain.Services;

namespace Lykke.Job.EthereumBridge.DomainServices.RabbitMq.Handlers
{
    public class WalletLinkingStatusChangeCompletedHandler : IWalletLinkingStatusChangeCompletedHandler
    {
        private readonly IOperationsService _operationsService;
        private readonly IBlockchainEncodingService _blockchainEncodingService;
        private readonly ISettingsService _settingsService;
        private readonly ILog _log;

        public WalletLinkingStatusChangeCompletedHandler(
            IOperationsService operationsService,
            IBlockchainEncodingService blockchainEncodingService,
            ISettingsService settingsService,
            ILogFactory logFactory)
        {
            _operationsService = operationsService;
            _blockchainEncodingService = blockchainEncodingService;
            _settingsService = settingsService;
            _log = logFactory.CreateLog(this);
        }

        public async Task HandleAsync(string operationId, string internalAddress, string publicAddress)
        {
            if (string.IsNullOrEmpty(operationId))
            {
                _log.Error(message: "WalletLinkingStatusChangeRequested event with missing operationId",
                context: new { internalAddress, publicAddress });
                return;
            }

            if (string.IsNullOrEmpty(internalAddress))
            {
                _log.Error(message: "WalletLinkingStatusChangeRequested event with missing internalAddress",
                    context: new {operationId, publicAddress});
                return;
            }

            var encodedData = string.IsNullOrEmpty(publicAddress)
                ? _blockchainEncodingService.EncodeUnLinkPublicAccountData(internalAddress)
                : _blockchainEncodingService.EncodeLinkPublicAccountData(internalAddress, publicAddress);

            await _operationsService.ExecuteOperationAsync(operationId, _settingsService.GetMasterWalletAddress(),
                _settingsService.GetPublicBlockchainGatewayContractAddress(), encodedData);
        }
    }
}
