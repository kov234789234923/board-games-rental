using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Server.Strategies
{
    public class StandardRentStrategy : IRentCalculatorStrategy
    {
        public decimal CalculateTotal(int days, decimal basePricePerDay)
        {
            return days * basePricePerDay;
        }
    }
}

