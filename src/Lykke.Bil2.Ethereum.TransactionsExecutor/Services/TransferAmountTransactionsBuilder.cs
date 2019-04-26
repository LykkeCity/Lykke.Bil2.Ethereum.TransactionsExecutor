using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Numerics;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common.Extensions;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Services
{
    public class TransferAmountTransactionsBuilder : ITransferAmountTransactionsBuilder
    {
        private readonly IEthereumApi _ethereumApi;

        public TransferAmountTransactionsBuilder(IEthereumApi ethereumApi)
        {
            _ethereumApi = ethereumApi;
        }

        public async Task<BuildTransactionResponse> BuildTransferAmountAsync(BuildTransferAmountTransactionRequest request)
        {
            // TODO: prepare data required for signing, return as base58 encoded string
            //
            // Throw:
            // - Lykke.Bil2.Contract.Common.RequestValidationException:
            //     if a transaction can’t be built with the given parameters and
            //     it will be never possible to build the transaction with exactly the same parameters.
            //
            // - Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions.SendingTransactionBuildingException:
            //     if a transaction cannot be built and the reason can be mapped
            //     to the Lykke.Bil2.Contract.TransactionsExecutor.SendingTransactionBuildingError
            //
            // - Lykke.Bil2.Sdk.Exceptions.OperationNotSupportedException:
            //     if "transfer amount" transactions model is not supported by the blockchain.
            //
            // - System.Exception:
            //     if there are any other errors.
            //     Likely a temporary issue with infrastructure or configuration,
            //     request should be repeated later.
            if (request.Transfers.Count != 1)
                throw new RequestValidationException("Invalid transfers count, must be exactly 1");

            if (request.Fees.Count != 3)
                throw new RequestValidationException("Invalid fees count, must be exactly 3");

            var transfer = request.Transfers.First();

            var transactionRaw = await _ethereumApi.BuildTransactionAsync(transfer, request.Fees);

            return new BuildTransactionResponse(transactionRaw.ToBase58());
        }
    }
}
