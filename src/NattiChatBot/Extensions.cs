using Microsoft.Extensions.Options;
using NattiChatBot.Domain;
using NattiChatBot.Domain.Enums;
using NattiChatBot.Options;

#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable RCS1110 // Declare types in namespaces
namespace NattiChatBot;

public static class Extensions
#pragma warning restore RCS1110 // Declare types in namespaces
#pragma warning restore CA1050 // Declare types in namespaces
{
    public static T GetConfiguration<T>(this IServiceProvider serviceProvider) where T : class
    {
        var o = serviceProvider.GetService<IOptions<T>>();
        if (o is null)
            throw new ArgumentNullException(nameof(T));

        return o.Value;
    }

    public static ControllerActionEndpointConventionBuilder MapBotWebhookRoute<T>(
        this IEndpointRouteBuilder endpoints,
        string route
    )
    {
        var controllerName = typeof(T).Name.Replace("Controller", "");
        var actionName = typeof(T).GetMethods()[0].Name;

        return endpoints.MapControllerRoute(
            name: "bot_webhook",
            pattern: route,
            defaults: new { controller = controllerName, action = actionName }
        );
    }

    public static string GetPfzToken(this HttpContext ctx)
    {
        return ctx.Request.Headers["X-Pfz-Token"]!;
    }
}