using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Server.Strategies
{
    public interface IRentCalculatorStrategy
    {
        // Метод принимает количество дней аренды и базовую цену проката в день
        decimal CalculateTotal(int days, decimal basePricePerDay);
    }
}


