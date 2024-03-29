using Microsoft.EntityFrameworkCore;
using NattiChatBot.Counter;
using NattiChatBot.Data;
using NattiChatBot.Data.Filters;
using NattiChatBot.Domain;
using NattiChatBot.Services.Interfaces;

namespace NattiChatBot.Services;

public class StatsService : IStatsService
{
    private readonly BotContext _db;
    private readonly ILogger<StatsService> _logger;

    public StatsService(BotContext db, ILogger<StatsService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<Stats>> GetAll(DateFilter dateFilter, CancellationToken ct)
    {
        var statsCollection = _db.Stats.AsQueryable();

        if (dateFilter.From is not null)
        {
            statsCollection = statsCollection.Where(x => x.Date >= dateFilter.From);
        }

        if (dateFilter.To is not null)
        {
            statsCollection = statsCollection.Where(x => x.Date <= dateFilter.To);
        }

        return await statsCollection.ToListAsync(ct);
    }

    public Stats GetCurrent()
    {
        var stats = new Stats(
            DateOnly.FromDateTime(DateTime.UtcNow + TimeSpan.FromHours(3)),
            Counters.NewMembersCount,
            Counters.MessagesCount
        );

        return stats;
    }

    public async Task Add(Stats stats, CancellationToken ct)
    {
        var existingStats = await _db.Stats
            .Where(x => x.Date == stats.Date)
            .SingleOrDefaultAsync(ct);

        if (existingStats is not null)
        {
            _logger.LogCritical("Stats for {StatsDate} were already added", stats.Date);
        }

        await _db.Stats.AddAsync(stats, ct);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Added stats for {StatsDate}, new members: {NewMembersCount}, messages: {MessagesCount}",
            stats.Date,
            stats.NewMembersCount,
            stats.MessagesCount
        );
    }
}