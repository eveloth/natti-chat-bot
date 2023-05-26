using NattiChatBot.Domain.Enums;

namespace NattiChatBot.Domain;

public record Token
{
    public Token()
    {
        AccessToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        IssuedAt = DateTime.UtcNow;
    }

    public Token(DateTime issuedAt, DateTime expiresAt, string grantedTo, AccessType accessType)
    {
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
        GrantedTo = grantedTo;
        AccessType = accessType;
    }

    public Token(
        DateTime issuedAt,
        DateTime expiresAt,
        string grantedTo,
        string accessToken,
        AccessType accessType
    )
        : this(issuedAt, expiresAt, grantedTo, accessType)
    {
        AccessToken = accessToken;
    }

    public long Id { get; set; }
    public string AccessToken { get; init; }
    public DateTime IssuedAt { get; init; }
    public DateTime ExpiresAt { get; set; }
    public string GrantedTo { get; set; }
    public AccessType AccessType { get; set; }

    public static Token GenerateDefaultToken(string accessToken, AccessType accessType)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMonths(3);

        return new Token(now, expires, "admin", accessToken, accessType);
    }
}