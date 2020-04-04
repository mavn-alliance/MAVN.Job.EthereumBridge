using MAVN.Job.EthereumBridge.Domain.Services;

namespace MAVN.Job.EthereumBridge.DomainServices.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly string _publicBlockchainGatewayContractAddress;
        private readonly string _publicBlockchainTokenContractAddress;
        private readonly string _masterWalletAddress;

        public SettingsService(
            string publicBlockchainGatewayContractAddress,
            string publicBlockchainTokenContractAddress,
            string masterWalletAddress)
        {
            _publicBlockchainGatewayContractAddress = publicBlockchainGatewayContractAddress;
            _publicBlockchainTokenContractAddress = publicBlockchainTokenContractAddress;
            _masterWalletAddress = masterWalletAddress;
        }

        public string GetMasterWalletAddress()
            => _masterWalletAddress;

        public string GetPublicBlockchainGatewayContractAddress()
            => _publicBlockchainGatewayContractAddress;
        
        public string GetPublicBlockchainTokenContractAddress()
            => _publicBlockchainTokenContractAddress;
    }
}
