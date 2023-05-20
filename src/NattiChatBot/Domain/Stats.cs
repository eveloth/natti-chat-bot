namespace NattiChatBot.Domain;

public record Stats(DateOnly Date, int NewMembersCount, int MessagesCount)
{
    public long Id { get; set; }
}