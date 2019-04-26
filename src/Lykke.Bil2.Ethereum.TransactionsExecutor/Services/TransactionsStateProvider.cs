using System.Threading.Tasks;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Services
{
    public class TransactionsStateProvider : ITransactionsStateProvider
    {
        private readonly IEthereumApi _ethereumApi;

        public TransactionsStateProvider(IEthereumApi ethereumApi)
        {
            _ethereumApi = ethereumApi;
        }

        public async Task<TransactionState> GetStateAsync(string transactionId)
        {
            var tx = await _ethereumApi.GetTransactionStateAsync(transactionId);

            return tx;
        }
    }
}
