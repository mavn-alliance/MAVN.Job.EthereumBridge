namespace MAVN.Job.EthereumBridge.Domain.Services
{
    public interface ISettingsService
    {
        string GetMasterWalletAddress();

        string GetPublicBlockchainGatewayContractAddress();

        string GetPublicBlockchainTokenContractAddress();
    }
}
