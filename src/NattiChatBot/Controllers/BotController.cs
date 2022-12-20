using Microsoft.AspNetCore.Mvc;
using NattiChatBot.Filters;
using NattiChatBot.Services;
using Telegram.Bot.Types;

namespace NattiChatBot.Controllers;

public class BotController : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post(
        [FromBody] Update update,
        [FromServices] UpdateHandlers handleUpdateService,
        CancellationToken cancellationToken)
    {
        await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
        return Ok();
    }
}
