using NattiChatBot.Data.Filters;
using NattiChatBot.Domain;

namespace NattiChatBot.Services.Interfaces;

public interface IStatsService
{
    Task<List<Stats>> GetAll(DateFilter dateFilter, CancellationToken ct);
    Task<Stats?> Get(DateOnly date, CancellationToken ct);
    Task Add(Stats stats, CancellationToken ct);
}