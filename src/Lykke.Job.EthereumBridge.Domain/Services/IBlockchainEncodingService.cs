using System.Numerics;
using Falcon.Numerics;

namespace Lykke.Job.EthereumBridge.Domain.Services
{
    public interface IBlockchainEncodingService
    {
        string EncodeTransferToExternalData(string privateAddress, string publicAddress, BigInteger internalTransferId, Money18 amount);
        string EncodeLinkPublicAccountData(string privateAddress, string publicAddress);
        string EncodeUnLinkPublicAccountData(string privateAddress);
        string EncodeSetInternalSupplyData(BigInteger newSupply);
    }
}
