using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Models;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;
using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Services
{
    public class DependencyInfoProvider : IDependenciesInfoProvider
    {
        public DependencyInfoProvider(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
        }

        public Task<IReadOnlyDictionary<DependencyName, DependencyInfo>> GetInfoAsync()
        {
            // TODO: If possible, return current and available new version (if any) of all components, of which current service is dependent.
            // For Example:
            //
            // var nodeAvailableVersion = ...;
            //
            // return new Dictionary<DependencyName, DependencyInfo>
            // {
            //     {
            //         "node", 
            //         new DependencyInfo(new Semver("1.2.3"), new Semver("1.4.2"))
            //     }
            // };

            throw new System.NotImplementedException();
        }
    }
}
