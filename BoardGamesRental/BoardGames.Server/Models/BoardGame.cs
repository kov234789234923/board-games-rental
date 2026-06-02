using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames.Server.Models
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
}

