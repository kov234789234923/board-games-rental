using System;

namespace BoardGames.Server.Models
{
    public class Rental
    {
        public int Id { get; set; }
        public int BoardGameId { get; set; }
        public BoardGame? BoardGame { get; set; }
        public int ClientId { get; set; }
        public Client? Client { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime PlannedReturnDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DepositPaid { get; set; }
        public string Status { get; set; } = "Активен"; // Активен, Завершен
    }
}
