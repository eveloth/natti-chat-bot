using Microsoft.Extensions.Options;
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
        ICounterAlertJob counterAlertJob, CommandExecutor commandExecutor)
    {
        _botClient = botClient;
        _logger = logger;
        _counterAlertJob = counterAlertJob;
        _commandExecutor = commandExecutor;
        _botConfig = botOptions.Value;
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

        var chatId = message.Chat.Id;

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

        var isAdmin =
            (
                await _botClient.GetChatMemberAsync(
                    message.Chat.Id,
                    sender.Id,
                    cancellationToken: cancellationToken
                )
            ).Status == ChatMemberStatus.Administrator;

        var isEntitled =
            _botConfig.EntitledUserId is { } entitledUserId && entitledUserId == sender.Id;

        var adminAction = messageText switch
        {
            "/enable_counter" => _counterAlertJob.EnableCounters(),
            "/disable_counter" => _counterAlertJob.DisableCounters(),
            _ => Task.CompletedTask
        };

        var regularAction = messageText switch
        {
            "/stats" => _commandExecutor.SendStats(message, cancellationToken),
            _ => Task.CompletedTask
        };

        if (isAdmin || isEntitled)
        {
            await adminAction;
            await regularAction;
        }
        else
        {
            await regularAction;
        }

        async Task WelcomeToTheUnion(
            ITelegramBotClient botClient,
            Message message,
            CancellationToken cancellationToken
        )
        {
            var sticker = new InputFileId(
                "CAACAgIAAxkBAAEbKPNjoO0h4FTT4cvD48JH5oiva1TfMgACwQADRvjVB5h6U1iKJsQ4LAQ"
            );
            await botClient.SendStickerAsync(
                chatId,
                sticker,
                replyToMessageId: message.MessageId,
                cancellationToken: cancellationToken
            );
        }
    }
}