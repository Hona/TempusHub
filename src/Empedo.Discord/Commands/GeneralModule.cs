using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Empedo.Discord.Helpers;

namespace Empedo.Discord.Commands
{
    public class GeneralModule : EmpedoModuleBase
    {
        [Command("stalktop")]
        [Aliases("topplayersonline", "st", "tpo")]
        public async Task TopPlayersOnlineAsync(CommandContext context)
        {
            var embeds = await TempusEmbedService.GetTopPlayersOnlineAsync();

            await context.RespondWithPaginatedEmbedsAsync(embeds);
        }
        
        [Command("rr")]
        public async Task RecentMapRecordsAsync(CommandContext context)
        {
            var embeds = await TempusEmbedService.GetRecentMapRecordsAsync();

            await context.RespondWithPaginatedEmbedsAsync(embeds);
        }
        
        [Command("rrc")]
        public async Task RecentCourseRecordsAsync(CommandContext context)
        {
            var embeds = await TempusEmbedService.GetRecentCourseRecordsAsync();

            await context.RespondWithPaginatedEmbedsAsync(embeds);
        }
        
        [Command("rrb")]
        public async Task RecentBonusRecordsAsync(CommandContext context)
        {
            var embeds = await TempusEmbedService.GetRecentBonusRecordsAsync();

            await context.RespondWithPaginatedEmbedsAsync(embeds);
        }
        
        [Command("rrtt")]
        public async Task RecentMapTopTimesAsync(CommandContext context)
        {
            var embeds = await TempusEmbedService.GetRecentMapTopTimesAsync();

            await context.RespondWithPaginatedEmbedsAsync(embeds);
        }

        [Command("map")]
        [Aliases("m")]
        public async Task Map(CommandContext context, string map)
        {
            var embeds = await TempusEmbedService.GetMapOverviewAsync(map);

            await context.RespondWithPaginatedEmbedsAsync(embeds);
        }
    }
}