using Microsoft.EntityFrameworkCore;
using NattiChatBot.Data.ModelConfigurations;
using NattiChatBot.Domain;

namespace NattiChatBot.Data;

public class BotContext : DbContext
{
    public BotContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StatsConfiguration).Assembly);
    }

    public DbSet<Stats> Stats { get; set; }
    public DbSet<Token> Tokens { get; set; }
}