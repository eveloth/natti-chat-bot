namespace NattiChatBot.Domain;

public record Stats(long Id, DateOnly Date, int NewMembersCount, int MessagesCount);