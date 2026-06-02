using BoardGames.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BoardGames.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Адрес в сети будет: http://localhost:5000/api/games
    public class GamesController : ControllerBase
    {
        private readonly BoardGamesContext _context = new();

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardGame>>> GetGames()
        {
            return await _context.BoardGames.Include(g => g.Category).ToListAsync();
        }
    }
}
