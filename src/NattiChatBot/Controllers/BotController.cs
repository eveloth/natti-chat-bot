using Microsoft.AspNetCore.Mvc;
using NattiChatBot.Filters;
using NattiChatBot.Services;
using Telegram.Bot.Types;

namespace NattiChatBot.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
public class BotController : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post(
        [FromBody] Update update,
        [FromServices] UpdateHandlers handleUpdateService,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
        }
        catch (Exception e)
        {
            await handleUpdateService.HandleErrorAsync(e);
        }

        return Ok();
    }
}