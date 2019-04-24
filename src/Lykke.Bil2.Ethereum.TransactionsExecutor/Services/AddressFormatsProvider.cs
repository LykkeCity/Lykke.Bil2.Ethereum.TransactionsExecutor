using System.Threading.Tasks;
using Lykke.Bil2.Contract.Common;
using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Services
{
    public class AddressFormatsProvider : IAddressFormatsProvider
    {
        public AddressFormatsProvider(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
        }

        public async Task<AddressFormatsResponse> GetFormatsAsync(string address)
        {
            // Return all formats of the address.
            //
            // Throw:
            // - Lykke.Bil2.Contract.Common.Exceptions.RequestValidationException:
            //     if input parameters are invalid.
            //
            // - Lykke.Bil2.Sdk.Exceptions.OperationNotSupportedException:
            //     if multiple address formats are not supported by the blockchain.
            //
            // - System.Exception:
            //     if there are any other errors.
            //     Likely a temporary issue with infrastructure or configuration,
            //     request should be repeated later.
            //
            // For example:
            //
            // Account account;
            //
            // try
            // {
            //     account = Account.CreateFromEncodedAddress(address);
            // }
            // catch
            // {
            //     throw new RequestValidationException("Invalid address", nameof(address));
            // }
            //
            // return new AddressFormatsResponse(new AddressFormat[]
            // {
            //     new AddressFormat(account.Address.Pretty, "Pretty"),
            //     new AddressFormat(account.Address.Plain, "Plain")
            // });


            throw new System.NotImplementedException();
        }
    }
}