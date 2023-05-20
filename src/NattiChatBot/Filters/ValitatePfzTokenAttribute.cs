using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using NattiChatBot.Domain;
using NattiChatBot.Domain.Enums;
using NattiChatBot.Services.Interfaces;

namespace NattiChatBot.Filters;

[AttributeUsage(AttributeTargets.Method)]
public class ValitatePfzTokenAttribute : Attribute, IAsyncActionFilter
{
    private readonly AccessType _accessType;

    public ValitatePfzTokenAttribute(AccessType accessType)
    {
        _accessType = accessType;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        var isTokenProvided = context.HttpContext.Request.Headers.TryGetValue(
            "X-Pfz-Token",
            out var secretTokenHeader
        );

        if (!isTokenProvided)
        {
            context.Result = new ObjectResult("\"X-Pfz-Token is not provided\"")
            {
                StatusCode = 403
            };
            return;
        }

        var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
        var token = await tokenService.Get(secretTokenHeader!, CancellationToken.None);

        var isTokenValid =
            token?.AccessType.HasFlag(_accessType) & token?.ExpiresAt > DateTime.UtcNow;

        if (isTokenValid is null or false)
        {
            context.Result = new ObjectResult("Insufficient priveleges or token is invalid")
            {
                StatusCode = 403
            };
            return;
        }
        await next();
    }
}