using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.SharedDomain;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Services
{
    public class DependencyInfoProvider : IDependenciesInfoProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IEthereumApi _ethereumApi;

        public DependencyInfoProvider(IEthereumApi ethereumApi, IHttpClientFactory httpClientFactory)
        {
            _ethereumApi = ethereumApi;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IReadOnlyDictionary<DependencyName, DependencyInfo>> GetInfoAsync()
        {
            string runningVersion = await _ethereumApi.GetRunningVersionAsync();
            var latestRelease = await _httpClientFactory.CreateClient(Startup.ParityGitHubRepository)
                .GetAsync("releases/latest");

            latestRelease.EnsureSuccessStatusCode();

            dynamic json = JsonConvert.DeserializeObject(await latestRelease.Content.ReadAsStringAsync());

            return new Dictionary<DependencyName, DependencyInfo>
            {
                ["node"] = new DependencyInfo(runningVersion, (string) json.tag_name)
            };

            throw new System.NotImplementedException();
        }
    }
}
