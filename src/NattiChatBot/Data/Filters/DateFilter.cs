namespace NattiChatBot.Data.Filters;

public record DateFilter(DateOnly? From, DateOnly? To);