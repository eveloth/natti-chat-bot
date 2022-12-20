using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace NattiChatBot.Services;

public class UpdateHandlers
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandlers> _logger;

    public UpdateHandlers(ITelegramBotClient botClient, ILogger<UpdateHandlers> logger)
    {
        _botClient = botClient;
        _logger = logger;
    }

    public Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", errorMessage);
        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        var handler = update switch
        {
            { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
            _ => Task.CompletedTask
        };

        await handler;
    }

    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Receive message type: {MessageType}", message.Type);

        if (message.NewChatMembers is not null)
        {
            await WelcomeToTheUnion(_botClient, message, cancellationToken);
        }

        static async Task WelcomeToTheUnion(
            ITelegramBotClient botClient,
            Message message,
            CancellationToken cancellationToken
        )
        {
            var sticker = new InputFileId(
                "CAACAgIAAxkBAAEbKPNjoO0h4FTT4cvD48JH5oiva1TfMgACwQADRvjVB5h6U1iKJsQ4LAQ"
            );
            await botClient.SendStickerAsync(
                message.Chat.Id,
                sticker,
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken
            );
        }
    }
}