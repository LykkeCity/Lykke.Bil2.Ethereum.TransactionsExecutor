using Lykke.Bil2.Contract.Common.Exceptions;
using Lykke.Bil2.Contract.TransactionsExecutor.Requests;
using Lykke.Bil2.Ethereum.TransactionsExecutor.Constants;
using Lykke.Bil2.Ethereum.TransactionsExecutor.Exceptions;
using Lykke.Bil2.SharedDomain;
using Lykke.Numerics;
using Nethereum.Contracts;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lykke.Bil2.Contract.TransactionsExecutor.Responses;
using Nethereum.RPC.Eth.Transactions;
using Polly;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Services
{
    public interface IEthereumApi
    {
        Task<string> BuildTransactionAsync(Transfer transfer, IEnumerable<Fee> fees);

        Task<EstimateTransactionResponse> EstimateTransferAsync(Transfer transfer);

        Task<TransactionState> GetTransactionStateAsync(string transactionId);

        Task<(long lastIrreversibleBlockNumber, DateTime lastIrreversibleBlockTime)>
            GetLastIrreversibleBlockAsync();

        Task<string> GetRunningVersionAsync();

        Task<bool> GetIsSynchronizedAsync();

        Task<string> BroadcastRawAsync(string rawTransacion);
    }

    public class EthereumApi : IEthereumApi
    {
        private readonly Web3 _ethClient;
        private BigInteger _minGasPrice;
        private BigInteger _maxGasPrice;
        private readonly int _confirmationBlocks;

        public EthereumApi(string parityUrl)
        {
            _confirmationBlocks = 30;
            _minGasPrice = new BigInteger(2000000000);
            _maxGasPrice = new BigInteger(10000000000);
            _ethClient = new Web3(parityUrl);
        }

        public async Task<string> BuildTransactionAsync(Transfer transfer, IEnumerable<Fee> fees)
        {
            Fee gasLimitFee = null;
            Fee gasPriceFee = null;
            var feeDict = fees.ToDictionary(x => x.Asset.Id, y => y);

            feeDict.TryGetValue(FeeAssets.GasLimitAsset.Id, out gasLimitFee);
            feeDict.TryGetValue(FeeAssets.GasPriceAsset.Id, out gasPriceFee);

            string from = transfer.SourceAddress;
            string to = transfer.DestinationAddress;
            BigInteger amount = transfer.Amount.Significand;
            BigInteger nonce = await GetNextNonceAsync(from);
            BigInteger gasPrice = gasPriceFee.Amount.Significand;
            BigInteger gasLimit = gasLimitFee.Amount.Significand;
            string data = "";
            Nethereum.Signer.Transaction transaction = null;

            if (transfer.Asset.Id == "ETH")
            {
                transaction = new Nethereum.Signer.Transaction(to, amount, nonce, gasPrice, gasLimit);
            }
            else
            {
                data = GetTransferFunctionCallEncoded(transfer.Asset.Address.Value, to, amount);
                transaction = new Nethereum.Signer.Transaction(transfer.Asset.Address.Value, 0, nonce, gasPrice, gasLimit, data);
            }

            string raw = transaction.GetRLPEncoded().ToHex();

            return raw;
        }

        public async Task<EstimateTransactionResponse> EstimateTransferAsync(Transfer transfer)
        {
            string from = transfer.SourceAddress;
            string to = transfer.DestinationAddress;
            BigInteger amount = transfer.Amount.Significand;
            BigInteger gasPrice = await GetCurrentGasPriceAsync();
            string transactionData = "";

            if (transfer.Asset.Id == "ETH")
            {
            }
            else
            {
                to = transfer.Asset.Address.Value;
                transactionData = GetTransferFunctionCallEncoded(to, transfer.DestinationAddress, amount);
                amount = 0;
            }

            return await EstimateTransactionExecutionCostAsync(from, to, amount, gasPrice, transactionData);
        }

        public async Task<(long lastIrreversibleBlockNumber, DateTime lastIrreversibleBlockTime)>
            GetLastIrreversibleBlockAsync()
        {
            var bestBlock = await _ethClient.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            ulong irreversibleBlockNumber = (ulong)(bestBlock.Value - _confirmationBlocks);
            var irreversibleBlock = await _ethClient
                .Eth
                .Blocks
                .GetBlockWithTransactionsHashesByNumber.SendRequestAsync(new BlockParameter(irreversibleBlockNumber));
            var dateTime = UnixTimeStampToDateTime((double)irreversibleBlock.Timestamp.Value);

            return ((long)irreversibleBlockNumber, dateTime);
        }

        private static readonly Regex VersionExtractorRegex = new Regex(@"v\d+[.]\d+[.]\d+");

        public async Task<string> GetRunningVersionAsync()
        {
            var version = await _ethClient.Client.SendRequestAsync<JValue>(new RpcRequest($"{Guid.NewGuid()}", "web3_clientVersion"));
            var versionStr = version.Value.ToString();
            var match = VersionExtractorRegex.Match(versionStr);

            return match.Value;
        }

        public async Task<bool> GetIsSynchronizedAsync()
        {
            var request = await _ethClient.Eth.Syncing.SendRequestAsync();

            //if request.IsSyncing == true then node is not synced yet!
            return !request.IsSyncing;
        }

        public async Task<string> BroadcastRawAsync(string rawTransacion)
        {
            var ethSendTransaction = new EthSendRawTransaction(_ethClient.Client);
            string transactionHex;

            try
            {
                transactionHex = await ethSendTransaction.SendRequestAsync(rawTransacion);
            }
            catch (Nethereum.JsonRpc.Client.RpcResponseException ex)
            {
                //Ensure it
                if (ex.Message == "intrinsic gas too low")
                    throw new Lykke.Bil2.Sdk.TransactionsExecutor.Exceptions.
                        TransactionBroadcastingException(TransactionBroadcastingError.TransientFailure, ex.Message);

                throw ex;
            }

            return transactionHex;
        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public async Task<TransactionState> GetTransactionStateAsync(string transactionId)
        {
            var isInThePool = await IsTransactionInPool(transactionId);

            if (isInThePool)
                return TransactionState.Accepted;

            var receipt = await _ethClient.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionId);
            if (receipt == null)
            {
                return TransactionState.Unknown;
            }

            return TransactionState.Mined;
        }

        private async Task<EstimateTransactionResponse> EstimateTransactionExecutionCostAsync(
            string fromAddress,
            string toAddress,
            BigInteger amount,
            BigInteger gasPrice,
            string transactionData)
        {
            TransactionEstimationFailure transactionEstimationFailure = null;
            var fromAddressBalance = await _ethClient.Eth.GetBalance.SendRequestAsync(fromAddress, BlockParameter.CreatePending());
            //var currentGasPrice = await _web3.Eth.GasPrice.SendRequestAsync();
            var value = new HexBigInteger(amount);
            CallInput callInput;
            HexBigInteger estimatedGas;
            bool isAllowed = true;
            callInput = new CallInput(transactionData, toAddress, value);
            callInput.From = fromAddress;

            try
            {
                if (await IsSmartContracAsync(toAddress))
                {
                    var callResult = await _ethClient.Eth.Transactions.Call.SendRequestAsync(callInput);
                    //Get amount of gas for smart contract execution
                    estimatedGas = await _ethClient.Eth.Transactions.EstimateGas.SendRequestAsync(callInput);
                }
                else
                {
                    estimatedGas = new HexBigInteger(21000);
                }

                //Recalculate transaction eth Amount
                var neededAmount = amount + estimatedGas.Value * gasPrice;
                var diff = fromAddressBalance - neededAmount;


                if (diff < 0)
                {
                    transactionEstimationFailure = new TransactionEstimationFailure(TransactionEstimationError.NotEnoughBalance,
                        $"{fromAddress} does not have enough ETH for transfer. Expected : {neededAmount}, Actual {fromAddressBalance.Value}");
                }

                //callInput.Value = new HexBigInteger(amount);
                //callInput.GasPrice = new HexBigInteger(gasPrice);

                ////reestimate with new arguments
                //estimatedGas = await _ethClient.Eth.Transactions.EstimateGas.SendRequestAsync(callInput);
            }
            catch (Nethereum.JsonRpc.Client.RpcResponseException rpcException)
            {
                var rpcError = rpcException?.RpcError;
                if (rpcError != null &&
                    rpcError.Code == -32000)
                {
                    throw new RequestValidationException(rpcException.Message);
                }
                else
                {
                    throw new Exception(rpcException.Message);
                }
            }
            catch (RequestValidationException e)
            {
                throw;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            var fees = new Fee[]
            {
                new Fee(FeeAssets.GasLimitAsset, UMoney.Create(estimatedGas, 0)),
                new Fee(FeeAssets.GasPriceAsset, UMoney.Create(gasPrice, 0)),
                new Fee(FeeAssets.EthAsset, UMoney.Create(estimatedGas * gasPrice, 18)),
            };

            var estimateTransactionResponse = new EstimateTransactionResponse(fees, transactionEstimationFailure);

            return estimateTransactionResponse;

            //    new OperationEstimation()
            //{
            //    GasAmount = estimatedGas.Value,
            //    GasPrice = gasPrice,
            //    EthAmount = amount,
            //    IsAllowed = isAllowed
            //};
        }

        private async Task<BigInteger> GetNextNonceAsync(string address)
        {
            var txPool = await _ethClient.Client.SendRequestAsync<JValue>(new RpcRequest($"{Guid.NewGuid()}", "parity_nextNonce", address));
            var bigInt = new HexBigInteger(txPool.Value.ToString());

            return bigInt.Value;
        }

        private Nethereum.Contracts.Contract GetContract(string erc20ContactAddress)
        {
            Nethereum.Contracts.Contract contract = _ethClient.Eth.GetContract(Erc20Constants.Abi, erc20ContactAddress);

            return contract;
        }

        private async Task<bool> IsSmartContracAsync(string toAddress)
        {
            var code = await _ethClient.Eth.GetCode.SendRequestAsync(toAddress);

            return code != "0x";
        }

        private async Task<BigInteger> GetCurrentGasPriceAsync()
        {
            BigInteger currentGasPrice = await _ethClient.Eth.GasPrice.SendRequestAsync();

            if (currentGasPrice < _minGasPrice)
                currentGasPrice = _minGasPrice;

            if (currentGasPrice > _maxGasPrice)
                currentGasPrice = _minGasPrice;

            return currentGasPrice;
        }

        private string GetTransferFunctionCallEncoded(string tokenAddress, string receiverAddress, BigInteger amount)
        {
            Nethereum.Contracts.Contract contract = GetContract(tokenAddress);
            Function transferFunction = contract.GetFunction("transfer");
            string functionDataEncoded = transferFunction.GetData(receiverAddress, amount);
            return functionDataEncoded;
        }

        public async Task<bool> IsTransactionInPool(string transactionHash)
        {
            Transaction transaction = await _ethClient.Eth.Transactions.GetTransactionByHash.SendRequestAsync(transactionHash);
            if (transaction == null || (transaction.BlockNumber.Value != 0))
            {
                return false;
            }

            return true;
        }
    }
}
