using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.SharedDomain;
using Nethereum.Util;
using System.Threading.Tasks;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Services
{
    public class AddressFormatsProvider : IAddressFormatsProvider
    {
        private readonly AddressUtil _addressUtil;

        public AddressFormatsProvider()
        {
            _addressUtil = new AddressUtil();
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

            string checkSumAddress;

            try
            {
                checkSumAddress = _addressUtil.ConvertToChecksumAddress(address);
            }
            catch
            {
                throw new RequestValidationException("Invalid address", nameof(address));
            }

            return new AddressFormatsResponse(new AddressFormat[]
            {
                 new AddressFormat(checkSumAddress, "Checksum"),
            });
        }
    }
}
