using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Client.Models
{
    public class BoardGame
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal PriceSale { get; set; }
        public decimal PriceRentPerDay { get; set; }
        public decimal DepositAmount { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
