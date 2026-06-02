using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Server.Strategies
{
    public class RentCalculator
    {
        private IRentCalculatorStrategy _strategy;

        
        public RentCalculator(IRentCalculatorStrategy strategy)
        {
            _strategy = strategy;
        }

        // Метод позволяет динамически менять тариф (стратегию) прямо во время работы
        public void SetStrategy(IRentCalculatorStrategy strategy)
        {
            _strategy = strategy;
        }

        
        public decimal ExecuteCalculate(int days, decimal basePricePerDay)
        {
            if (days <= 0) return 0;
            return _strategy.CalculateTotal(days, basePricePerDay);
        }
    }
}
