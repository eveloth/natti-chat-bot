using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NattiChatBot.Data;
using NattiChatBot.Domain;
using NattiChatBot.Options;

namespace NattiChatBot;

public class Seeder
{
    private readonly BotContext _db;
    private readonly ILogger<Seeder> _logger;
    private readonly DefaultAdminTokenOptions _defaultToken;

    public Seeder(
        BotContext db,
        ILogger<Seeder> logger,
        IOptions<DefaultAdminTokenOptions> defaultTokenOptions
    )
    {
        _db = db;
        _logger = logger;
        _defaultToken = defaultTokenOptions.Value;
    }

    public async Task InitializeDefaultToken()
    {
        var doTokensExist = await _db.Tokens.AnyAsync();

        if (doTokensExist)
        {
            _logger.LogInformation("Tokens exist, skipping seeding");
            return;
        }

        _logger.LogInformation("No tokens exist. Initializing tokens database");

        var token = Token.GenerateDefaultToken(
            _defaultToken.AccessToken,
            Enum.Parse<AccessType>(_defaultToken.AccessType)
        );

        await _db.Tokens.AddAsync(token);
        await _db.SaveChangesAsync();

        _logger.LogInformation("Default token added successfully");
    }
}