namespace NattiChatBot.Counter;

public static class Counters
{
    private static int _messagesCount;
    private static int _newMembersCount;

    public static bool Enabled { get; set; }
    public static int MessagesCount
    {
        get => _messagesCount;
        set
        {
            if (Enabled)
            {
                _messagesCount = value;
            }
        }
    }

    public static int NewMembersCount
    {
        get => _newMembersCount;
        set
        {
            if (Enabled)
            {
                _newMembersCount = value;
            }
        }
    }

    public static void ResetCounters()
    {
        _messagesCount = 0;
        _newMembersCount = 0;
    }
}