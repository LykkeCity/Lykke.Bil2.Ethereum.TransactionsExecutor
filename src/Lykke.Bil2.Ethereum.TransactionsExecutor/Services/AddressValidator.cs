using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.SharedDomain;
using Nethereum.Util;
using System.Threading.Tasks;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Services
{
    public class AddressValidator : IAddressValidator
    {
        private readonly AddressUtil _addressUtil;

        public AddressValidator(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
            _addressUtil = new AddressUtil();
        }

        public Task<AddressValidityResponse> ValidateAsync(string address, AddressTagType? tagType = null, string tag = null)
        {
            // TODO: validate address and, optionally, tag type and tag
            //
            // Throw
            // - Lykke.Bil2.Contract.Common.RequestValidationException:
            //     if input parameters are invalid and result can't be mapped to
            //     any of Lykke.Bil2.Contract.TransactionsExecutor.AddressValidationResult value.
            //
            // - System.Exception:
            //     if there are any other errors.
            //     Likely a temporary issue with infrastructure or configuration,
            //     request should be repeated later.
            //
            // For example, for shared deposit wallet and numeric tag approach (used in Ripple f.e.) validation may look like:
            //

            var isAddressFormatValid = !string.IsNullOrEmpty(address) 
                                       && _addressUtil.IsValidEthereumAddressHexFormat(address);

            var result = !isAddressFormatValid
                ? new AddressValidityResponse(AddressValidationResult.InvalidAddressFormat)
                : new AddressValidityResponse(AddressValidationResult.Valid);

            return Task.FromResult(result);
        }
    }
}
