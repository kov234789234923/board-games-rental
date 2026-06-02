using System;

namespace BoardGames.Server.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int BoardGameId { get; set; }
        public BoardGame? BoardGame { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal SalePrice { get; set; }
    }
}
