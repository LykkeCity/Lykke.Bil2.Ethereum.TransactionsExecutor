using System;
using System.Numerics;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Exceptions
{
    public class NotEnoughFundsException : Exception
    {
        public NotEnoughFundsException(string address, BigInteger expectedAmount, BigInteger actualAmount) :
            base($"{address} does not have enough ETH for transfer. Expected : {expectedAmount}, Actual {actualAmount}")
        {
        }
    }
}
