using Microsoft.Extensions.Options;
using NattiChatBot.Counter;
using NattiChatBot.Jobs;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NattiChatBot.Services;

public class UpdateHandlers
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandlers> _logger;
    private readonly BotConfiguration _botConfig;
    private readonly ICounterAlertJob _counterAlertJob;
    private readonly CommandExecutor _commandExecutor;

    public UpdateHandlers(
        ITelegramBotClient botClient,
        ILogger<UpdateHandlers> logger,
        IOptions<BotConfiguration> botOptions,
        ICounterAlertJob counterAlertJob,
        CommandExecutor commandExecutor
    )
    {
        _botClient = botClient;
        _logger = logger;
        _counterAlertJob = counterAlertJob;
        _commandExecutor = commandExecutor;
        _botConfig = botOptions.Value;
    }

    public Task HandleErrorAsync(Exception exception)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogError("HandleError: {ErrorMessage}", errorMessage);
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
        var chatId = message.Chat.Id;

        _logger.LogInformation("Received a new message from chat ID {ChatId}", chatId);

        if (chatId != _botConfig.ChatId)
        {
            return;
        }

        Counters.MessagesCount++;

        if (message.NewChatMembers is not null)
        {
            await WelcomeToTheUnion(_botClient, message, cancellationToken);
            Counters.NewMembersCount++;
            return;
        }

        if (message.Text is not { } messageText || message.From is not { } sender)
        {
            return;
        }

        if (await IsAdminOrIsEntitled(chatId, sender.Id, cancellationToken))
        {
            var adminAction = messageText switch
            {
                "/enable_counter" => _counterAlertJob.EnableCounters(),
                "/disable_counter" => _counterAlertJob.DisableCounters(),
                "/stats" => _commandExecutor.SendStats(message, cancellationToken),
                _ => Task.CompletedTask
            };

            await adminAction;

            return;
        }

        var regularAction = messageText switch
        {
            "/stats" => _commandExecutor.SendStats(message, cancellationToken),
            _ => Task.CompletedTask
        };

        await regularAction;
    }

    private static async Task WelcomeToTheUnion(
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

    private async Task<bool> IsAdminOrIsEntitled(
        long chatId,
        long userId,
        CancellationToken cancellationToken
    )
    {
        var isAdmin =
            (
                await _botClient.GetChatMemberAsync(
                    chatId,
                    userId,
                    cancellationToken: cancellationToken
                )
            ).Status == ChatMemberStatus.Administrator;

        var isEntitled =
            _botConfig.EntitledUserId is { } entitledUserId && entitledUserId == userId;

        return isAdmin || isEntitled;
    }
}
