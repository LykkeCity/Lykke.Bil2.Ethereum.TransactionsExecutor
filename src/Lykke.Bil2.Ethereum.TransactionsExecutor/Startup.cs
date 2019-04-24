using System;
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
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.DependenciesInfoProviderFactory = ctx =>
                    new DependencyInfoProvider
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.BlockchainInfoProviderFactory = ctx =>
                    new BlockchainInfoProvider
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.TransactionBroadcasterFactory = ctx =>
                    new TransactionBroadcaster
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.TransactionsStateProviderFactory = ctx =>
                    new TransactionsStateProvider
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.TransferAmountTransactionsBuilderFactory = ctx =>
                    new TransferAmountTransactionsBuilder
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );

                options.TransferAmountTransactionsEstimatorFactory = ctx =>
                    new TransferAmountTransactionsEstimator
                    (
                        /* TODO: Provide specific settings and dependencies, if necessary */
                    );


                // To access settings for any purpose,
                // usually, to register additional services like blockchain client,
                // uncomment code below:
                //
                // options.UseSettings = settings =>
                // {
                //     services.AddSingleton<IService>(new ServiceImpl(settings.CurrentValue.ServiceSettingValue));
                // };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseBlockchainTransactionsExecutor(options =>
            {
                options.IntegrationName = IntegrationName;
            });
        }
    }
}
