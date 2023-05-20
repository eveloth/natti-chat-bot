using NattiChatBot.Domain;

namespace NattiChatBot.Services.Interfaces;

public interface ITokenService
{
    Task<List<Token>> GetAll(CancellationToken ct);

    Task<Token?> Get(string token, CancellationToken ct);
    Task<Token> Add(Token token, CancellationToken ct);
    Task<Token> Update(Token token, CancellationToken ct);
    Task Delete(long id, CancellationToken ct);
}