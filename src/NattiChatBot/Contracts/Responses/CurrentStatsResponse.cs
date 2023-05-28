namespace NattiChatBot.Contracts.Responses;

public record CurrentStatsResponse(DateOnly Date, int NewMembersCount, int MessagesCount);