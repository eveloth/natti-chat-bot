namespace NattiChatBot.Domain;

[Flags]
public enum AccessType
{
    User = 0,
    Admin = 1,
    Full = 2
}