using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using BoardGames.Server.Models;
using BoardGames.Server.Strategies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace BoardGames.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly BoardGamesContext _context = new();
        private readonly string _connectionString = @"Server=(localdb)\mssqllocaldb;Database=BoardGamesDB;Trusted_Connection=True;";

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardGame>>> GetGames([FromQuery] string? titleFilter, [FromQuery] bool useOrm = true)
        {
            if (useOrm)
            {
                var query = _context.BoardGames.Include(g => g.Category).AsQueryable();
                if (!string.IsNullOrEmpty(titleFilter))
                {
                    query = query.Where(g => g.Title.Contains(titleFilter));
                }
                return await query.ToListAsync();
            }
            else
            {
                var games = new List<BoardGame>();
                string sqlQuery = @"SELECT g.Id, g.Title, g.PriceSale, g.PriceRentPerDay, g.DepositAmount, g.CategoryId, c.Name 
                                    FROM BoardGames g
                                    LEFT JOIN Categories c ON g.CategoryId = c.Id";

                if (!string.IsNullOrEmpty(titleFilter))
                {
                    sqlQuery += " WHERE g.Title LIKE @filter";
                }

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        if (!string.IsNullOrEmpty(titleFilter))
                        {
                            cmd.Parameters.AddWithValue("@filter", $"%{titleFilter}%");
                        }

                        await conn.OpenAsync();
                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                games.Add(new BoardGame
                                {
                                    Id = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    PriceSale = reader.GetDecimal(2),
                                    PriceRentPerDay = reader.GetDecimal(3),
                                    DepositAmount = reader.GetDecimal(4),
                                    CategoryId = reader.GetInt32(5),
                                    Category = new Category
                                    {
                                        Id = reader.GetInt32(5),
                                        Name = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                                    }
                                });
                            }
                        }
                    }
                }
                return Ok(games);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] BoardGame game, [FromQuery] bool useOrm = true)
        {
            if (useOrm)
            {
                _context.BoardGames.Add(game);
                await _context.SaveChangesAsync();
                return Ok(game);
            }
            else
            {
                string sqlQuery = @"INSERT INTO BoardGames (Title, PriceSale, PriceRentPerDay, DepositAmount, CategoryId) 
                                    VALUES (@Title, @PriceSale, @PriceRentPerDay, @DepositAmount, @CategoryId);
                                    SELECT CAST(scope_identity() AS int);";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", game.Title);
                        cmd.Parameters.AddWithValue("@PriceSale", game.PriceSale);
                        cmd.Parameters.AddWithValue("@PriceRentPerDay", game.PriceRentPerDay);
                        cmd.Parameters.AddWithValue("@DepositAmount", game.DepositAmount);
                        cmd.Parameters.AddWithValue("@CategoryId", game.CategoryId);

                        await conn.OpenAsync();
                        game.Id = (int)await cmd.ExecuteScalarAsync();
                    }
                }
                return Ok(game);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGame(int id, [FromBody] BoardGame game, [FromQuery] bool useOrm = true)
        {
            if (useOrm)
            {
                var existingGame = await _context.BoardGames.FindAsync(id);
                if (existingGame == null) return NotFound("Игра не найдена");

                existingGame.Title = game.Title;
                existingGame.PriceSale = game.PriceSale;
                existingGame.PriceRentPerDay = game.PriceRentPerDay;
                existingGame.DepositAmount = game.DepositAmount;
                existingGame.CategoryId = game.CategoryId;

                await _context.SaveChangesAsync();
                return Ok(existingGame);
            }
            else
            {
                string sqlQuery = @"UPDATE BoardGames 
                                    SET Title = @Title, PriceSale = @PriceSale, PriceRentPerDay = @PriceRentPerDay, DepositAmount = @DepositAmount, CategoryId = @CategoryId 
                                    WHERE Id = @Id";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);
                        cmd.Parameters.AddWithValue("@Title", game.Title);
                        cmd.Parameters.AddWithValue("@PriceSale", game.PriceSale);
                        cmd.Parameters.AddWithValue("@PriceRentPerDay", game.PriceRentPerDay);
                        cmd.Parameters.AddWithValue("@DepositAmount", game.DepositAmount);
                        cmd.Parameters.AddWithValue("@CategoryId", game.CategoryId);

                        await conn.OpenAsync();
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected == 0) return NotFound("Игра не найдена");
                    }
                }
                return Ok(game);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id, [FromQuery] bool useOrm = true)
        {
            if (useOrm)
            {
                var game = await _context.BoardGames.FindAsync(id);
                if (game == null) return NotFound("Игра не найдена");

                _context.BoardGames.Remove(game);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            else
            {
                string sqlQuery = "DELETE FROM BoardGames WHERE Id = @Id";

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", id);

                        await conn.OpenAsync();
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected == 0) return NotFound("Игра не найдена");
                    }
                }
                return NoContent();
            }
        }
        [HttpPost("sell/{id}")]
        public async Task<IActionResult> SellGame(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string gameQuery = "SELECT Id, Title, PriceSale FROM BoardGames WHERE Id = @id";
                int gameId = 0;
                string gameTitle = "";
                decimal priceSale = 0;

                using (SqlCommand cmd = new SqlCommand(gameQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync()) return NotFound("Игра не найдена");
                        gameId = reader.GetInt32(0);
                        gameTitle = reader.GetString(1);
                        priceSale = reader.GetDecimal(2);
                    }
                }

                string clientQuery = "SELECT TOP(1) Id, FullName FROM Clients";
                int clientId = 0;
                string clientName = "";

                using (SqlCommand cmd = new SqlCommand(clientQuery, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync()) return BadRequest("Нет зарегистрированных клиентов");
                        clientId = reader.GetInt32(0);
                        clientName = reader.GetString(1);
                    }
                }

                string insertQuery = @"INSERT INTO Orders (BoardGameId, ClientId, OrderDate, SalePrice) 
                                       VALUES (@bgId, @clId, @date, @price)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@bgId", gameId);
                    cmd.Parameters.AddWithValue("@clId", clientId);
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@price", priceSale);

                    await cmd.ExecuteNonQueryAsync();
                }

                return Ok(new { message = $"Игра '{gameTitle}' успешно продана клиенту {clientName}! (Режим: Чистый SQL)" });
            }
        }

        [HttpPost("rent/{id}")]
        public async Task<IActionResult> RentGame(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                string gameQuery = "SELECT Id, Title, PriceRentPerDay, DepositAmount FROM BoardGames WHERE Id = @id";
                int gameId = 0;
                string gameTitle = "";
                decimal priceRentPerDay = 0;
                decimal depositAmount = 0;

                using (SqlCommand cmd = new SqlCommand(gameQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync()) return NotFound("Игра не найдена");
                        gameId = reader.GetInt32(0);
                        gameTitle = reader.GetString(1);
                        priceRentPerDay = reader.GetDecimal(2);
                        depositAmount = reader.GetDecimal(3);
                    }
                }

                string clientQuery = "SELECT TOP(1) Id FROM Clients";
                int clientId = 0;

                using (SqlCommand cmd = new SqlCommand(clientQuery, conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (!await reader.ReadAsync()) return BadRequest("Нет зарегистрированных клиентов");
                        clientId = reader.GetInt32(0);
                    }
                }

                var rentCalculator = new RentCalculator(new StandardRentStrategy());
                int rentalDays = 3;
                decimal finalRentPrice = rentCalculator.ExecuteCalculate(rentalDays, priceRentPerDay);

                string insertQuery = @"INSERT INTO Rentals (BoardGameId, ClientId, IssueDate, PlannedReturnDate, TotalPrice, DepositPaid, Status) 
                                       VALUES (@bgId, @clId, @issue, @planned, @total, @deposit, @status)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@bgId", gameId);
                    cmd.Parameters.AddWithValue("@clId", clientId);
                    cmd.Parameters.AddWithValue("@issue", DateTime.Now);
                    cmd.Parameters.AddWithValue("@planned", DateTime.Now.AddDays(rentalDays));
                    cmd.Parameters.AddWithValue("@total", finalRentPrice);
                    cmd.Parameters.AddWithValue("@deposit", depositAmount);
                    cmd.Parameters.AddWithValue("@status", "Активен");

                    await cmd.ExecuteNonQueryAsync();
                }

                return Ok(new { message = $"Игра '{gameTitle}' успешно выдана в прокат на {rentalDays} дня! Стоимость: {finalRentPrice} руб. Залог: {depositAmount} руб. (Режим: Чистый SQL)" });
            }
        }
    }
}
