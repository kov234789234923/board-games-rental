using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Server.Strategies
{
    public class WeekendRentStrategy : IRentCalculatorStrategy
    {
        public decimal CalculateTotal(int days, decimal basePricePerDay)
        {
            decimal fullPrice = days * basePricePerDay;
            return fullPrice * 1.20m; // Наценка 20%
        }
    }
}

