using NattiChatBot.Counter;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NattiChatBot.Services;

public class CommandExecutor
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<CommandExecutor> _logger;

    public CommandExecutor(ITelegramBotClient botClient, ILogger<CommandExecutor> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public async Task SendStats(Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing command {Command}", nameof(SendStats));

        var replyText =
            "Статистика выполнения плана по профсоюзу на данный момент:\n\n"
            + $"\u2709 {Counters.MessagesCount} сообщений\n"
            + $"\U0001f195 {Counters.NewMembersCount} новых участников\n\n"
            + "<a href=\"https://natttti.vercel.app/\">Посмотреть на дашборде</a>";

        await _botClient.SendTextMessageAsync(
            message.Chat.Id,
            replyText,
            replyToMessageId: message.MessageId,
            cancellationToken: cancellationToken
        );
    }
}