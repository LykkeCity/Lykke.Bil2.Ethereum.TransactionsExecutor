using System;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Lykke.Bil2.Sdk.TransactionsExecutor.Services;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Services
{
    public class BlockchainInfoProvider : IBlockchainInfoProvider
    {
        public BlockchainInfoProvider(/* TODO: Provide specific settings and dependencies, if necessary */)
        {
        }

        public Task<BlockchainInfo> GetInfoAsync()
        {
            // TODO: Provide current blockchain state (last confirmed block number and time).
            //
            // var info = ...;
            //
            // return new BlockchainInfo(info.LastIrreversibleBlockNumber, info.LastIrreversibleBlockTime);

            throw new NotImplementedException();
        }
    }
}