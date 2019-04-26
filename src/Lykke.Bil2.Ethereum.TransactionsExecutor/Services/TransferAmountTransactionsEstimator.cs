using System.Linq;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.SharedDomain;
using Lykke.Numerics;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Ethereum.TransactionsExecutor.Constants;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Services
{
    public class TransferAmountTransactionsEstimator : ITransferAmountTransactionsEstimator
    {
        private readonly IEthereumApi _ethereumApi;

        public TransferAmountTransactionsEstimator(IEthereumApi ethereumApi)
        {
            _ethereumApi = ethereumApi;
        }

        public async Task<EstimateTransactionResponse> EstimateTransferAmountAsync(EstimateTransferAmountTransactionRequest request)
        {
            // TODO: estimate transaction fees.
            //
            // Throw:
            // - Lykke.Bil2.Contract.Common.RequestValidationException:
            //     if a transaction can’t be estimated with the given parameters
            //     and it will be never possible to estimate the transaction with exactly the same
            //     parameters.
            //
            // - System.Exception:
            //     if there are any other errors.
            //     Likely a temporary issue with infrastructure or configuration,
            //     request should be repeated later.
            //
            // Result usually consists of single fee element,
            // but for some blockchains fees may be charged in multiple currencies/tokens.

            if (request.Transfers.Count != 1)
                throw new RequestValidationException("Amount of transfer should be equal to 1");

            var transfer = request.Transfers.First();

            var transactionResponse = await _ethereumApi.EstimateTransferAsync(transfer);

            return transactionResponse;
        }
    }
}
