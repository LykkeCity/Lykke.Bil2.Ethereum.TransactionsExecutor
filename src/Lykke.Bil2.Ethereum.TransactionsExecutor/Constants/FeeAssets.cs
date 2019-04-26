using Lykke.Bil2.SharedDomain;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Constants
{
    public static class FeeAssets
    {
        public static readonly Asset EthAsset = new Asset(new AssetId("ETH"));

        public static readonly Asset GasLimitAsset = new Asset(new AssetId("ETH_GasLimit"));

        public static readonly Asset GasPriceAsset = new Asset(new AssetId("ETH_GasPrice"));
    }
}
