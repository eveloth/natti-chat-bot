using Microsoft.EntityFrameworkCore;
using NattiChatBot.Data;

namespace NattiChatBot.Installers;

public static class PersistenceInstaller
{
    public static void InstallPersistenceLayer(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<BotContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
        });

        builder.Services.AddScoped<Seeder>();
    }
}