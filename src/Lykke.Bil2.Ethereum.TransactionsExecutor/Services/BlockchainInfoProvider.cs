using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using System.Threading.Tasks;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Services
{
    public class BlockchainInfoProvider : IBlockchainInfoProvider
    {
        private readonly IEthereumApi _ethereumApi;

        public BlockchainInfoProvider(IEthereumApi ethereumApi)
        {
            _ethereumApi = ethereumApi;
        }

        public async Task<BlockchainInfo> GetInfoAsync()
        {
            var (lastIrreversibleBlockNumber, 
                lastIrreversibleBlockTime) = await _ethereumApi.GetLastIrreversibleBlockAsync();

            return new BlockchainInfo(lastIrreversibleBlockNumber, lastIrreversibleBlockTime);
        }
    }
}
