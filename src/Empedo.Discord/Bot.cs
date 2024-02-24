using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using Microsoft.Extensions.Hosting;

namespace Empedo.Discord
{
    public class Bot : IHostedService
    {
        private readonly DiscordClient _discordClient;
        
        public Bot(DiscordClient discordClient)
        {
            _discordClient = discordClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _discordClient.ConnectAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _discordClient.DisconnectAsync();
        }
    }
}