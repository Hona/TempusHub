using System;
using System.Net.Http;
using Blazored.LocalStorage;
using Empedo.Discord;
using Empedo.Discord.Commands;
using Empedo.Discord.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using Serilog;
using Serilog.Events;
using TempusApi;
using TempusHub.Application.Services;
using TempusHub.Core.Models;
using TempusHub.Infrastructure;
using TempusHub.Web.HostedServices;

var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console();

var seqUri = Environment.GetEnvironmentVariable("SEQ_URI");
var seqApiKey = Environment.GetEnvironmentVariable("SEQ_API_KEY");

if (!string.IsNullOrWhiteSpace(seqUri) && !string.IsNullOrWhiteSpace(seqApiKey))
{
    loggerConfiguration
        .WriteTo.Seq(seqUri, apiKey: seqApiKey, restrictedToMinimumLevel: LogEventLevel.Debug);
}

Log.Logger = loggerConfiguration.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

// Add services to the container
var services = builder.Services;

services.AddRazorPages();
services.AddServerSideBlazor();
services.AddControllers();

var forwardedHeaderOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedHeaderOptions.KnownNetworks.Clear();
forwardedHeaderOptions.KnownProxies.Clear();
services.AddSingleton(forwardedHeaderOptions);

services.AddSingleton<TempusHubMySqlService>();
services.AddSingleton<ITempusClient, TempusClient>(_ => new TempusClient(new HttpClient()));
services.AddSingleton<TempusCacheService>();
services.AddSingleton<TempusRecordCacheService>();
services.AddSingleton<YoutubeApiService>();
services.AddHostedService<CacheHostedService>();
services.AddSingleton(Log.Logger);

services.Configure<TempusHubConfig>(builder.Configuration.GetSection("TempusHub"));

services.AddMudServices();
        
services.AddBlazoredLocalStorage();

services.AddSwaggerGen();

services.AddEmpedoDiscordBot(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseForwardedHeaders();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "TempusHub API V1"); });

app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllers();

try
{
    Log.Information("Starting web host");
    
    app.Services.InitializeMicroservices(typeof(EmpedoModuleBase).Assembly);
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}        