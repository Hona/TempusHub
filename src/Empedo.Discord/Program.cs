using System.Reflection;
using DSharpPlus;
using Empedo.Discord;
using Empedo.Discord.Helpers;
using Empedo.Discord.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using TempusApi;

using var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();

var logFactory = new LoggerFactory().AddSerilog(logger);

var hostBuilder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        
    });

var host = hostBuilder.Build();

host.Services.InitializeMicroservices(Assembly.GetEntryAssembly());

host.Run();