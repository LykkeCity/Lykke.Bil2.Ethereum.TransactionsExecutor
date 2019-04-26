using System;
using System.Net.Http;
using JetBrains.Annotations;
using Lykke.Bil2.Ethereum.TransactionsExecutor.Services;
using Lykke.Bil2.Ethereum.TransactionsExecutor.Settings;
using Lykke.Bil2.Sdk.TransactionsExecutor;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor
{
    [UsedImplicitly]
    public class Startup
    {
        public const string ParityGitHubRepository = "ParityGitHubRepository";
        private const string IntegrationName = "Ethereum";

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            return services.BuildBlockchainTransactionsExecutorServiceProvider<AppSettings>(options =>
            {
                options.IntegrationName = IntegrationName;

                // Register required service implementations:

                options.AddressFormatsProviderFactory = ctx =>
                    new AddressFormatsProvider
                    (
                    /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.AddressValidatorFactory = ctx =>
                    new AddressValidator
                    (
                    /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.HealthProviderFactory = ctx =>
                    new HealthProvider
                    (
                    ctx.Services.GetRequiredService<IEthereumApi>()
                    );

                options.DependenciesInfoProviderFactory = ctx =>
                    new DependencyInfoProvider
                    (
                        ctx.Services.GetRequiredService<IEthereumApi>(),
                        ctx.Services.GetRequiredService<IHttpClientFactory>()
                    );

                options.BlockchainInfoProviderFactory = ctx =>
                    new BlockchainInfoProvider
                    (
                        ctx.Services.GetRequiredService<IEthereumApi>()
                    );

                options.TransactionBroadcasterFactory = ctx =>
                    new TransactionBroadcaster
                    (
                        ctx.Services.GetRequiredService<IEthereumApi>()
                    );

                options.TransactionsStateProviderFactory = ctx =>
                    new TransactionsStateProvider
                    (
                        ctx.Services.GetRequiredService<IEthereumApi>()
                    );

                options.TransferAmountTransactionsBuilderFactory = ctx =>
                    new TransferAmountTransactionsBuilder
                    (
                        ctx.Services.GetRequiredService<IEthereumApi>()
                    );

                options.TransferAmountTransactionsEstimatorFactory = ctx =>
                    new TransferAmountTransactionsEstimator
                    (
                        ctx.Services.GetRequiredService<IEthereumApi>()
                    );


                // To access settings for any purpose,
                // usually, to register additional services like blockchain client,
                // uncomment code below:

                options.UseSettings = (serviceCollection, settings) =>
                {
                    serviceCollection.AddTransient<IEthereumApi>();

                    serviceCollection.AddHttpClient(ParityGitHubRepository)
                        .ConfigureHttpClient(client =>
                        {
                            client.BaseAddress = new Uri("https://api.github.com/repos/paritytech/parity-ethereum/");
                            client.DefaultRequestHeaders.Add("User-Agent", "Lykke.Bil2.Ethereum");
                        });
                };
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseBlockchainTransactionsExecutor(options =>
            {
                options.IntegrationName = IntegrationName;
            });
        }
    }
}
