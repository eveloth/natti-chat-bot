using Hangfire;
using Microsoft.Extensions.Options;
using NattiChatBot.Counter;
using NattiChatBot.Domain;
using NattiChatBot.Services.Interfaces;
using Telegram.Bot;

namespace NattiChatBot.Jobs;

public class CounterAlertJob : ICounterAlertJob
{
    private readonly ITelegramBotClient _botClient;
    private readonly BotConfiguration _botConfig;
    private readonly ILogger<CounterAlertJob> _logger;
    private readonly IServiceProvider _serviceProvider;

    public CounterAlertJob(
        ITelegramBotClient botClient,
        IOptions<BotConfiguration> botOptions,
        ILogger<CounterAlertJob> logger,
        IServiceProvider serviceProvider
    )
    {
        _botClient = botClient;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _botConfig = botOptions.Value;
    }

    public async Task EnableCounters()
    {
        if (Counters.Enabled)
        {
            return;
        }

        Counters.Enabled = true;
        await _botClient.SendTextMessageAsync(
            _botConfig.ChatId,
            "Ура, товарищи!\n"
                + "Теперь я буду каждый день объявлять вам, "
                + "сколько человек присоединилось к нашему профсоюзу "
                + "и сколько сообщений вы написали!"
        );

        RecurringJob.AddOrUpdate("alert", () => SendAlert(), "0 21 * * *");
        _logger.LogInformation(
            "Started counter at {DateTime} for chat ID {ChatId}",
            DateTime.Now,
            _botConfig.ChatId
        );
    }

    public async Task DisableCounters()
    {
        if (!Counters.Enabled)
        {
            return;
        }

        Counters.Enabled = false;
        await _botClient.SendTextMessageAsync(
            _botConfig.ChatId,
            "Увы, товарищи! Пока что больше никаких объявлений."
        );

        RecurringJob.RemoveIfExists("alert");
        Counters.ResetCounters();
        _logger.LogInformation(
            "Stopped counter at {DateTime} for chat ID {ChatId}",
            DateTime.Now,
            _botConfig.ChatId
        );
    }

    public async Task SendAlert()
    {
        var newMembers = Counters.NewMembersCount;
        var messages = Counters.MessagesCount;
        var now = DateTime.UtcNow;

        await _botClient.SendTextMessageAsync(
            _botConfig.ChatId,
            "Так держать, товарищи!\n "
                + $"Сегодня вы написали {messages} сообщений, "
                + $"и нас стало больше на {newMembers} человек!"
        );

        using (var scope = _serviceProvider.CreateScope())
        {
            var statsService = scope.ServiceProvider.GetRequiredService<IStatsService>();

            var stats = new Stats(DateOnly.FromDateTime(now), newMembers, messages);
            await statsService.Add(stats, CancellationToken.None);
        }

        _logger.LogInformation(
            "Sent alert: {NewMembers} new members, {Messages} messages at {DateTime}",
            newMembers,
            messages,
            now
        );

        Counters.ResetCounters();
    }
}