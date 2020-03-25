using System.Numerics;
using System.Threading.Tasks;
using Common.Log;
using Falcon.Numerics;
using Lykke.Common.Log;
using Lykke.Job.EthereumBridge.Domain.RabbitMq.Handlers;
using Lykke.Job.EthereumBridge.Domain.Services;

namespace Lykke.Job.EthereumBridge.DomainServices.RabbitMq.Handlers
{
    public class TransferToExternalProcessedHandler : ITransferToExternalProcessedHandler
    {
        private readonly IOperationsService _operationsService;
        private readonly IBlockchainEncodingService _blockchainEncodingService;
        private readonly ISettingsService _settingsService;
        private readonly ILog _log;

        public TransferToExternalProcessedHandler(
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

        public async Task HandleAsync(
            string privateAddress,
            string publicAddress,
            BigInteger internalTransferId,
            string operationId,
            Money18 amount)
        {
            #region Validation

            if (string.IsNullOrEmpty(privateAddress))
            {
                _log.Error(message: "TransferToExternalProcessed event with missing private address",
                    context: new {operationId, internalTransferId, publicAddress, amount});
                return;
            }

            if (string.IsNullOrEmpty(publicAddress))
            {
                _log.Error(message: "TransferToExternalProcessed event with missing public address",
                    context: new { operationId, internalTransferId, privateAddress, amount });
                return;
            }


            if (string.IsNullOrEmpty(operationId))
            {
                _log.Error(message: "TransferToExternalProcessed event with missing operation Id",
                    context: new { publicAddress, privateAddress, internalTransferId, amount });
                return;
            }

            if (amount <= 0)
            {
                _log.Error(message: "TransferToExternalProcessed event with invalid amount ",
                    context: new { operationId, publicAddress, privateAddress, internalTransferId, amount });
                return;
            }

            #endregion

            var encodedData =
                _blockchainEncodingService.EncodeTransferToExternalData(privateAddress, publicAddress, internalTransferId, amount);

            await _operationsService.ExecuteOperationAsync(operationId, _settingsService.GetMasterWalletAddress(),
                _settingsService.GetPublicBlockchainGatewayContractAddress(), encodedData);
        }
    }
}
