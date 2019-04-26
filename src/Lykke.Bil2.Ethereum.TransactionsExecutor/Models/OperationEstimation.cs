using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace Lykke.Bil2.Ethereum.TransactionsExecutor.Models
{
    public class OperationEstimation
    {
        public BigInteger GasAmount { get; set; }

        public BigInteger GasPrice { get; set; }

        public BigInteger EthAmount { get; set; }

        public bool IsAllowed { get; set; }
    }
}
