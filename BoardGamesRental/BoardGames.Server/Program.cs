using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using BoardGames.Server;
using BoardGames.Server.Models;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddDbContext<BoardGamesContext>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BoardGamesContext>();

    
    if (!context.Categories.Any())
    {
        var strategyCategory = new Category { Name = "Стратегии" };
        var partyCategory = new Category { Name = "Пати-геймы" };

        context.Categories.AddRange(strategyCategory, partyCategory);

        context.BoardGames.AddRange(
            new BoardGame { Title = "Монополия", PriceSale = 2500, PriceRentPerDay = 150, DepositAmount = 1000, Category = strategyCategory },
            new BoardGame { Title = "Мафия", PriceSale = 900, PriceRentPerDay = 50, DepositAmount = 400, Category = partyCategory },
            new BoardGame { Title = "Каркассон", PriceSale = 2100, PriceRentPerDay = 120, DepositAmount = 800, Category = strategyCategory }
        );
    }

    
    if (!context.Clients.Any())
    {
        context.Clients.Add(new Client
        {
            FullName = "Иванов Иван Иванович",
            Phone = "+79991112233",
            Email = "ivanov@mail.ru",
            PassportData = "4508 123456"
        });
    }

    context.SaveChanges();
}

app.MapControllers();
app.Run("http://localhost:5000");
