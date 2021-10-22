using System;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using Serilog;
using Serilog.Events;
using TempusHubBlazor.Data;
using TempusHubBlazor.Services;

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

builder.WebHost.UseSerilog();

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

var tempusHubMySqlService = new TempusHubMySqlService(Environment.GetEnvironmentVariable("MYSQL_CONNECTION_STRING"));
services.AddSingleton(tempusHubMySqlService);
var tempusDataService = new TempusDataService(tempusHubMySqlService);
if (Environment.GetEnvironmentVariable("CACHE_ALL_RECORDS")?.ToLower() == "true")
{
    tempusDataService.CacheAllWRsAsync().GetAwaiter().GetResult();
}
services.AddSingleton(tempusDataService);
// Causes the cache to run immediately
services.AddSingleton(new TempusCacheService(tempusDataService));
services.AddSingleton<YoutubeApiService>();

services.AddMudServices();
        
services.AddBlazoredLocalStorage();

services.AddSwaggerGen();

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