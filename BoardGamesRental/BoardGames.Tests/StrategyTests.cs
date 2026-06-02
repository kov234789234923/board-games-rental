using BoardGames.Server.Strategies;

namespace BoardGames.Tests
{
    public class StrategyTests
    {
        [Fact]
        public void StandardStrategy_ShouldCalculateCorrectly()
        {
            
            var calculator = new RentCalculator(new StandardRentStrategy());
            int days = 5;
            decimal pricePerDay = 200m; 

            
            decimal result = calculator.ExecuteCalculate(days, pricePerDay);

            
            Assert.Equal(1000m, result);
        }

        [Fact]
        public void VipStrategy_ShouldApply15PercentDiscount()
        {
            
            var calculator = new RentCalculator(new VipRentStrategy());

            
            decimal result = calculator.ExecuteCalculate(5, 200m);

            
            Assert.Equal(850m, result);
        }

        [Fact]
        public void WeekendStrategy_ShouldApply20PercentMarkup()
        {
            
            var calculator = new RentCalculator(new WeekendRentStrategy());

            
            decimal result = calculator.ExecuteCalculate(5, 200m);

            
            Assert.Equal(1200m, result);
        }
    }
}
