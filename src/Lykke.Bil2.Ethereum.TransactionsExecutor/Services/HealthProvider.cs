using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Services
{
    public class HealthProvider : IHealthProvider
    {
        private readonly IEthereumApi _ethereumApi;

        public HealthProvider(IEthereumApi ethereumApi)
        {
            _ethereumApi = ethereumApi;
        }

        public async Task<string> GetDiseaseAsync()
        {
            // TODO: check state of node and/or service and return description of problems, if any.
            //
            // It is a good place to check if node is synchronized, if possible.
            // For example:

            try
            {
                if (!await _ethereumApi.GetIsSynchronizedAsync())
                {
                    return "Node is not synchronized";
                }

                return null;
            }
            catch (HttpRequestException ex)
            {
                return $"Node is unavailable: {ex}";
            }
        }
    }
}
