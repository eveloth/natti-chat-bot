namespace NattiChatBot.Domain;

public record Token(DateTime IssuedAt, DateTime ExpiresAt, string GrantedTo, AccessType AccessType)
{
    public Token(
        DateTime issuedAt,
        DateTime expiresAt,
        string grantedTo,
        string accessToken,
        AccessType accessType
    ) : this(issuedAt, expiresAt, grantedTo, accessType)
    {
        AccessToken = accessToken;
    }

    public long Id { get; init; }
    public string AccessToken { get; init; } = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

    public static Token GenerateDefaultToken(string accessToken, AccessType accessType)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMonths(3);

        return new Token(now, expires, "admin", accessToken, accessType);
    }
}