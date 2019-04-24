# Blockchain integration transaction executor

This component of blockchain integration is intended for building and executing transactions.

To implement transaction executor component:

1. Consider implementing dedicated service to provide an abstraction of blockchain client API.
   Such approach helps to write well structured and testable component.

2. Implement `IAddressFormatsProvider` if multiple address formats are supported by blockchain.

3. Implement `IAddressValidator` interface for validating deposit and withdraw addresses.

4. Implement `IHealthProvider` interface for checking services state.

5. Implement `IIntegrationInfoService` interface for checking blockchain state.

6. Implement `ITransactionBroadcaster` interface for broadcasting transactions.

7. Implement `ITransferAmountTransactionsBuilder` and `ITransferAmountTransactionsEstimator` interfaces
   for building and estimating transactions if `transfer amount` model is used in blockchain
   (usually such blockchains are JSON-based).

8. Implement `ITransferCoinsTransactionsBuilder` and `ITransferCoinsTransactionsEstimator` interfaces
   for building and estimating transactions if `transfer coins` model is used in blockchain
   (UTXO-based blockchain).

9. Provide implementations to SDK code in `ConfigureServices()` method on startup.  Use `options.UseSettings` delegate to register additional services in DI container.

10. Provide integration name and other options in `Configure()` method on startup.