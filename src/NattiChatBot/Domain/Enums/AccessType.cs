namespace NattiChatBot.Domain.Enums;

[Flags]
public enum AccessType
{
    User = 1,
    Admin = 2,
    Full = User | Admin
}