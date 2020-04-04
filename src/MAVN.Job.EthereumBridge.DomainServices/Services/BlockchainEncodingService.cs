using System.Numerics;
using Falcon.Numerics;
using MAVN.Job.EthereumBridge.Domain.Services;
using Nethereum.ABI.FunctionEncoding;
using Nethereum.Contracts;

namespace MAVN.Job.EthereumBridge.DomainServices.Services
{
    public class BlockchainEncodingService : IBlockchainEncodingService
    {
        private readonly FunctionCallEncoder _functionCallEncoder;

        public BlockchainEncodingService()
        {
            _functionCallEncoder = new FunctionCallEncoder();
        }

        public string EncodeTransferToExternalData(string privateAddress, string publicAddress, BigInteger internalTransferId, Money18 amount)
        {
            var func = new TransferFromInternalNetworkFunction
            {
                InternalAccount = privateAddress,
                PublicAccount = publicAddress,
                Amount = amount.ToAtto(),
                InternalTransferId = internalTransferId,
            };

            return EncodeRequestData(func);
        }

        public string EncodeLinkPublicAccountData(string privateAddress, string publicAddress)
        {
            var func = new LinkPublicAccountFunction
            {
                InternalAccount = privateAddress,
                PublicAccount = publicAddress,
            };
            return EncodeRequestData(func);
        }

        public string EncodeUnLinkPublicAccountData(string privateAddress)
        {
            var func = new UnlinkPublicAccountFunction
            {
                InternalAccount = privateAddress
            };
            return EncodeRequestData(func);
        }

        public string EncodeSetInternalSupplyData(BigInteger newSupply)
        {
            var func = new SetInternalSupplyFunction {NewInternalSupply = newSupply};

            return EncodeRequestData(func);
        }

        private string EncodeRequestData<T>(T func)
            where T : class, new()
        {
            var abiFunc = ABITypedRegistry.GetFunctionABI<T>();
            var result = _functionCallEncoder.EncodeRequest(func, abiFunc.Sha3Signature);

            return result;
        }
    }
}
