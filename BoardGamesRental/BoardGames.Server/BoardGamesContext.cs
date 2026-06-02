using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using BoardGames.Server.Models;

namespace BoardGames.Server
{
    public class BoardGamesContext : DbContext
    {
        public DbSet<BoardGame> BoardGames { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Client> Clients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=BoardGamesDB;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Entity<BoardGame>().Property(b => b.PriceSale).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BoardGame>().Property(b => b.PriceRentPerDay).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<BoardGame>().Property(b => b.DepositAmount).HasColumnType("decimal(18,2)");

            base.OnModelCreating(modelBuilder);
        }
    }
}


