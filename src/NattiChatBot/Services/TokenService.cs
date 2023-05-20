using Microsoft.EntityFrameworkCore;
using NattiChatBot.Data;
using NattiChatBot.Domain;
using NattiChatBot.Exceptions;
using NattiChatBot.Services.Interfaces;

namespace NattiChatBot.Services;

public class TokenService : ITokenService
{
    private readonly BotContext _db;
    private readonly ILogger<TokenService> _logger;

    public TokenService(BotContext db, ILogger<TokenService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<Token>> GetAll(CancellationToken ct)
    {
        return await _db.Tokens.ToListAsync(ct);
    }

    public async Task<Token?> Get(string token, CancellationToken ct)
    {
        return await _db.Tokens.Where(x => x.AccessToken == token).SingleOrDefaultAsync(ct);
    }

    public async Task<Token> Add(Token token, CancellationToken ct)
    {
        var existingToken = await _db.Tokens
            .Where(x => x.AccessToken == token.AccessToken)
            .SingleOrDefaultAsync(ct);

        if (existingToken is not null)
        {
            throw new ApiException("Token already exists");
        }

        await _db.Tokens.AddAsync(token, ct);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Added token ID {TokenId} granted to {GrantedTo} with {AccessType} access type",
            token.Id,
            token.GrantedTo,
            token.AccessType
        );

        return token;
    }

    public async Task<Token> Update(Token token, CancellationToken ct)
    {
        var existingToken = await _db.Tokens.FindAsync(token.Id, ct);

        if (existingToken is null)
        {
            throw new ApiException("Token not found");
        }

        existingToken.ExpiresAt = token.ExpiresAt;
        existingToken.GrantedTo = token.GrantedTo;
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Updated token ID {TokenId}. Expiry: {Expiry}, granted to: {GrantedTo}, access type {AccessType}",
            token.Id,
            token.ExpiresAt,
            token.GrantedTo,
            token.AccessType
        );

        return existingToken;
    }

    public async Task Delete(long id, CancellationToken ct)
    {
        var existingToken = await _db.Tokens.FindAsync(id, ct);

        if (existingToken is null)
        {
            return;
        }

        _db.Tokens.Remove(existingToken);
        await _db.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Token ID {TokenId} was deleted from {GrantedTo}",
            existingToken.Id,
            existingToken.GrantedTo
        );
    }
}