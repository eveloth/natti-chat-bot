using Telegram.Bot;
using Telegram.Bot.Types;

namespace NattiChatBot.Services;

public class CommandExecutor
{
    private readonly ITelegramBotClient _botClient;

    public CommandExecutor(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public async Task SendStats(Message message, CancellationToken cancellationToken)
    {
        var replyText =
            "Статистика выполнения плана по профсоюзу на данный момент:\n\n"
            + $"\u2709 {Counters.MessagesCount} сообщений\n"
            + $"\U0001f195 {Counters.NewMembersCount} новых участников";

        await _botClient.SendTextMessageAsync(
            message.Chat.Id,
            replyText,
            replyToMessageId: message.MessageId,
            cancellationToken: cancellationToken
        );
    }
}