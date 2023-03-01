namespace NattiChatBot.Counter;

public class CounterBackup
{
    public int MessagesCount { get; set; }
    public int NewMembersCount { get; set; }

    public CounterBackup(int messagesCount, int newMembersCount)
    {
        MessagesCount = messagesCount;
        NewMembersCount = newMembersCount;
    }
}