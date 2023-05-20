using Hangfire;
using Hangfire.Redis;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using NattiChatBot;
using NattiChatBot.Controllers;
using NattiChatBot.Data;
using NattiChatBot.Installers;
using NattiChatBot.Jobs;
using NattiChatBot.Options;
using NattiChatBot.Services;
using Serilog;
using StackExchange.Redis;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddEnvironmentVariables();

builder.InstallSerilog();

// Setup Bot configuration
var botConfigurationSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
builder.Services.Configure<BotConfiguration>(botConfigurationSection);

var adminTokenConfiguration = builder.Configuration.GetSection(
    DefaultAdminTokenOptions.DefaultAdminToken
);
builder.Services.Configure<DefaultAdminTokenOptions>(adminTokenConfiguration);

var botConfiguration = botConfigurationSection.Get<BotConfiguration>();

// Register named HttpClient to get benefits of IHttpClientFactory
// and consume it with ITelegramBotClient typed client.
// More read:
//  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#typed-clients
//  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
builder.Services
    .AddHttpClient("telegram_bot_client")
    .AddTypedClient<ITelegramBotClient>(
        (httpClient, sp) =>
        {
            BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
            TelegramBotClientOptions options = new(botConfig.BotToken);
            return new TelegramBotClient(options, httpClient);
        }
    );

var options = ConfigurationOptions.Parse(
    builder.Configuration.GetConnectionString("RedisHangfire")!
);
options.Password = builder.Configuration["Redis:Password"];
var redis = ConnectionMultiplexer.Connect(options);
builder.Services.AddSingleton(redis);

builder.InstallPersistenceLayer();

builder.Services.AddHangfire(configuration =>
{
    configuration.UseRedisStorage(redis, new RedisStorageOptions { Db = 5, });
});

builder.Services.AddHangfireServer();

builder.Services.AddScoped<UpdateHandlers>();
builder.Services.AddScoped<CommandExecutor>();
builder.Services.AddScoped<ICounterAlertJob, CounterAlertJob>();

// There are several strategies for completing asynchronous tasks during startup.
// Some of them could be found in this article https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
// We are going to use IHostedService to add and later remove Webhook
builder.Services.AddHostedService<ConfigureWebhook>();
builder.Services.AddHostedService<RedisBackupService>();

// The Telegram.Bot library heavily depends on Newtonsoft.Json library to deserialize
// incoming webhook updates and send serialized responses back.
// Read more about adding Newtonsoft.Json to ASP.NET Core pipeline:
//   https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-6.0#add-newtonsoftjson-based-json-format-support
builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();

app.UseForwardedHeaders(
    new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    }
);

using (var scope = app.Services.CreateScope())
{
    var botContext = scope.ServiceProvider.GetRequiredService<BotContext>();
    botContext.Database.Migrate();
}

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<Seeder>();

    try
    {
        await seeder.InitializeDefaultToken();
    }
    catch (Exception)
    {
        Log.Fatal("DATABASE SEEDING FAILED");
        throw;
    }
}

app.UseSerilogRequestLogging();

app.UseHangfireDashboard();

// Construct webhook route from the Route configuration parameter
// It is expected that BotController has single method accepting Update
app.MapBotWebhookRoute<BotController>(route: botConfiguration.Route);
app.MapControllers();
app.MapHangfireDashboard();

await app.RunAsync();

#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable RCS1110 // Declare type inside namespace.
namespace NattiChatBot
{
    public class BotConfiguration
#pragma warning restore RCS1110 // Declare type inside namespace.
#pragma warning restore CA1050 // Declare types in namespaces
    {
        public static readonly string Configuration = "BotConfiguration";

        public string BotToken { get; init; } = default!;
        public string HostAddress { get; init; } = default!;
        public string Route { get; init; } = default!;
        public string SecretToken { get; init; } = default!;
        public long ChatId { get; init; }
        public long? EntitledUserId { get; init; }
    }
}