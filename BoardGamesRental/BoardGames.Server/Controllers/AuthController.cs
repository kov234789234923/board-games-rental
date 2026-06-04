using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BoardGames.Server.Services;
using BoardGames.Server.Models;
using System.Threading.Tasks;

namespace BoardGames.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BoardGamesContext _context = new();

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
            if (user == null) return Unauthorized("Неверный логин или пароль.");

            if (!PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Unauthorized("Неверный логин или пароль.");
            }

            return Ok(new { Message = "Авторизация успешна", Username = user.Login });
        }
    }

    public class LoginRequest
    {
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
