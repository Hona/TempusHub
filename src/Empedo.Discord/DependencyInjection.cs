using System.Reflection;
using DSharpPlus;
using Empedo.Discord.Commands;
using Empedo.Discord.Helpers;
using Empedo.Discord.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Empedo.Discord;

public static class DependencyInjection
{
    public static void AddEmpedoDiscordBot(this IServiceCollection services, IConfiguration config)
    {
        var logFactory = new LoggerFactory().AddSerilog(Log.Logger);

        services.AddSingleton(new DiscordClient(new DiscordConfiguration
        {
            Token = config["Discord:Token"],
            TokenType = TokenType.Bot,
            AutoReconnect = true,
            MinimumLogLevel = LogLevel.None,
            LoggerFactory = logFactory,
            MessageCacheSize = 512,
            Intents = DiscordIntents.All
        }));

        services.InjectBotServices(typeof(EmpedoModuleBase).Assembly);
        
        services.AddSingleton<ITempusEmbedService, CachedTempusEmbedService>();

        services.AddHostedService<LambdaHostedService>();
        services.AddHostedService<Bot>();
    }
}