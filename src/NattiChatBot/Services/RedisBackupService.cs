using System.Text.Json;
using NattiChatBot.Counter;
using StackExchange.Redis;

namespace NattiChatBot.Services;

public class RedisBackupService : IHostedService
{
    private readonly ILogger<RedisBackupService> _logger;
    private readonly ConnectionMultiplexer _multiplexer;

    public RedisBackupService(ILogger<RedisBackupService> logger, ConnectionMultiplexer multiplexer)
    {
        _logger = logger;
        _multiplexer = multiplexer;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Checking for a backup");

        var db = _multiplexer.GetDatabase();
        var serializedBackup = await db.StringGetAsync("backup");

        if (!serializedBackup.HasValue)
        {
            _logger.LogInformation("Backup not found, skipping");
            return;
        }

        var backup = JsonSerializer.Deserialize<CounterBackup>(serializedBackup!);

        if (backup is null)
        {
            _logger.LogWarning("Could not deserialize backup, app will not restore its state");
            return;
        }

        _logger.LogInformation(
            "Found backup, new messages: {NewMessages}, new members: {NewMembers}",
            backup.MessagesCount,
            backup.NewMembersCount
        );

        Counters.Enabled = true;
        Counters.MessagesCount = backup.MessagesCount;
        Counters.NewMembersCount = backup.NewMembersCount;
        Counters.Enabled = false;
        _logger.LogInformation("App restored its state, proceeding");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating counters backup");

        var db = _multiplexer.GetDatabase();
        var backup = new CounterBackup(Counters.MessagesCount, Counters.NewMembersCount);

        await db.StringSetAsync("backup", JsonSerializer.Serialize(backup));
        _logger.LogInformation("Created counters backup, shutting down now");
    }
}