using NattiChatBot.Data.Filters;
using NattiChatBot.Domain;

namespace NattiChatBot.Services.Interfaces;

public interface IStatsService
{
    Task<List<Stats>> GetAll(DateFilter dateFilter, CancellationToken ct);
    Stats GetCurrent();
    Task Add(Stats stats, CancellationToken ct);
}