using System.Threading.Tasks;
using MAVN.Job.EthereumBridge.Domain.Repositories;
using MAVN.Job.EthereumBridge.MsSqlRepositories.Entities;
using MAVN.Persistence.PostgreSQL.Legacy;
using Microsoft.EntityFrameworkCore;

namespace MAVN.Job.EthereumBridge.MsSqlRepositories.Repositories
{
    public class NoncesRepository : INoncesRepository
    {
        private readonly IDbContextFactory<EthereumBridgeContext> _contextFactory;

        public NoncesRepository(IDbContextFactory<EthereumBridgeContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<long> GetNextNonceAsync(string masterWalletAddress, TransactionContext txContext)
        {
            using (var context = _contextFactory.CreateDataContext(txContext))
            {
                var nonce = await context.Nonces.FirstOrDefaultAsync(x => x.MasterWalletAddress == masterWalletAddress);
                if (nonce == null)
                {
                    nonce = NonceEntity.Create(masterWalletAddress, 0);
                    context.Add(nonce);
                }
                else
                {
                    nonce.Nonce += 1;
                    context.Update(nonce);
                }

                await context.SaveChangesAsync();

                return nonce.Nonce;
            }
        }
    }
}
