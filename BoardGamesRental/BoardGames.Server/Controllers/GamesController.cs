using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoardGames.Server.Models;
using BoardGames.Server.Strategies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BoardGames.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly BoardGamesContext _context = new();

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardGame>>> GetGames()
        {
            return await _context.BoardGames.Include(g => g.Category).ToListAsync();
        }

        
        [HttpPost("sell/{id}")]
        public async Task<IActionResult> SellGame(int id)
        {
            var game = await _context.BoardGames.FindAsync(id);
            if (game == null) return NotFound("Игра не найдена");

            var client = await _context.Clients.FirstOrDefaultAsync();
            if (client == null) return BadRequest("Нет зарегистрированных клиентов");

            var order = new Order
            {
                BoardGameId = game.Id,
                ClientId = client.Id,
                OrderDate = DateTime.Now,
                SalePrice = game.PriceSale
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Игра '{game.Title}' успешно продана клиенту {client.FullName}!" });
        }

        
        [HttpPost("rent/{id}")]
        public async Task<IActionResult> RentGame(int id)
        {
            var game = await _context.BoardGames.FindAsync(id);
            if (game == null) return NotFound("Игра не найдена");

            var client = await _context.Clients.FirstOrDefaultAsync();
            if (client == null) return BadRequest("Нет зарегистрированных клиентов");

            
            var rentCalculator = new RentCalculator(new StandardRentStrategy());
            int rentalDays = 3; 
            decimal finalRentPrice = rentCalculator.ExecuteCalculate(rentalDays, game.PriceRentPerDay);

            var rental = new Rental
            {
                BoardGameId = game.Id,
                ClientId = client.Id,
                IssueDate = DateTime.Now,
                PlannedReturnDate = DateTime.Now.AddDays(rentalDays),
                TotalPrice = finalRentPrice,
                DepositPaid = game.DepositAmount,
                Status = "Активен"
            };

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Игра '{game.Title}' успешно выдана в прокат на {rentalDays} дня! Стоимость: {finalRentPrice} руб. Залог: {game.DepositAmount} руб." });
        }
    }
}
