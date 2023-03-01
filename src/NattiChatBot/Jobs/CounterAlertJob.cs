using Hangfire;
using Microsoft.Extensions.Options;
using NattiChatBot.Counter;
using Telegram.Bot;

namespace NattiChatBot.Jobs;

public class CounterAlertJob : ICounterAlertJob
{
    private readonly ITelegramBotClient _botClient;
    private readonly BotConfiguration _botConfig;
    private readonly ILogger<CounterAlertJob> _logger;

    public CounterAlertJob(
        ITelegramBotClient botClient,
        IOptions<BotConfiguration> botOptions,
        ILogger<CounterAlertJob> logger
    )
    {
        _botClient = botClient;
        _logger = logger;
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
        await _botClient.SendTextMessageAsync(
            _botConfig.ChatId,
            "Так держать, товарищи!\n"
                + $"Сегодня вы написали {Counters.MessagesCount} сообщений, "
                + $"и нас стало больше на {Counters.NewMembersCount} человек!"
        );

        _logger.LogInformation(
            "Sent alert: {NewMembers} new members, {Messages} messages at {DateTime}",
            Counters.NewMembersCount,
            Counters.MessagesCount,
            DateTime.Now
        );

        Counters.ResetCounters();
    }
}