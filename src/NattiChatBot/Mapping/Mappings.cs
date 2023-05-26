using Mapster;
using NattiChatBot.Contracts.Queries;
using NattiChatBot.Data.Filters;

namespace NattiChatBot.Mapping;

public static class Mappings
{
    public static void ConfigureMapping(this IApplicationBuilder app)
    {
        TypeAdapterConfig<StatsFilterQuery, DateFilter>
            .NewConfig()
            .IgnoreNullValues(true)
            .Map(dest => dest.From, src => GetDateOnly(src.From))
            .Map(dest => dest.To, src => GetDateOnly(src.To));
    }

    private static DateOnly? GetDateOnly(DateTime? dateTime)
    {
        return dateTime.HasValue ? DateOnly.FromDateTime(dateTime.Value) : null;
    }
}