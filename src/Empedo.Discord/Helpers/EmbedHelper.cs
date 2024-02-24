using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace Empedo.Discord.Helpers
{
    public static class EmbedHelper
    {
        public static List<List<string>> SplitEmbedDescription(this List<string> lines, int lengthAllowed = 2048)
        {
            var output = new List<List<string>>();

            var currentEmbedLength = 0;
            var currentEmbed = new List<string>();
            
            foreach (var line in lines)
            {
                // Include the newline in the count
                var lineLength = line.Length + 2;
                currentEmbedLength += lineLength;

                if (currentEmbedLength > lengthAllowed)
                {
                    // Make a copy
                    output.Add(currentEmbed.ToList());
                    currentEmbed = new List<string>();
                    currentEmbedLength = lineLength;
                }
                
                currentEmbed.Add(line);
            }

            if (currentEmbed.Any())
            {
                output.Add(currentEmbed);
            }
            
            return output;
        }

        public static async Task SendAll(this IEnumerable<DiscordEmbedBuilder> embedBuilders, DiscordChannel discordChannel)
        {
            foreach (var embedBuilder in embedBuilders)
            {
                await discordChannel.SendMessageAsync(embedBuilder);
            }
        }

        public static List<DiscordEmbedBuilder> BuildEmbeds(this List<List<string>> lineGroups,
            Action<DiscordEmbedBuilder> firstEmbedAction, Action<DiscordEmbedBuilder> lastEmbedAction,
            bool runActionsEveryTime = false, string joinStringOverride = null)
        {
            var output = new List<DiscordEmbedBuilder>();

            for (var i = 0; i < lineGroups.Count; i++)
            {
                var lineGroup = lineGroups[i];
                
                var embed = new DiscordEmbedBuilder
                {
                    Description = string.Join(joinStringOverride ?? Environment.NewLine, lineGroup)
                };

                if (i == 0 || runActionsEveryTime)
                {
                    firstEmbedAction.Invoke(embed);
                }

                if (i == lineGroups.Count - 1 || runActionsEveryTime)
                {
                    lastEmbedAction.Invoke(embed);
                }

                output.Add(embed);
            }

            return output;
        }

        public static List<DiscordEmbedBuilder> BuildEmbeds<T>(this IEnumerable<T> model, Func<T, string> selector,
            Action<DiscordEmbedBuilder> firstEmbedAction, Action<DiscordEmbedBuilder> lastEmbedAction,
            bool runActionsEveryTime = false, string joinStringOverride = null)
        {
            var lines = model.Select(selector).ToList();

            var lineGroups = lines.SplitEmbedDescription();

            return lineGroups.BuildEmbeds(firstEmbedAction, lastEmbedAction, runActionsEveryTime, joinStringOverride);
        }

        private static T FindNonDefaultValue<T>(this List<DiscordEmbedBuilder> embeds,
            Func<DiscordEmbedBuilder, T> propertySelector)
        {
            var embedWithNonDefault = embeds.FirstOrDefault(x => propertySelector.Invoke(x) != null);

            return embedWithNonDefault == null ? default : propertySelector.Invoke(embedWithNonDefault);
        }

        public static List<Page> Paginate(this List<DiscordEmbedBuilder> embeds)
        {
            // If cached, each embed may not have the title (as per the 5min loop), so force set them
            // when paginating
            var title = embeds.FindNonDefaultValue(x => x.Title);
            var timestamp = embeds.FindNonDefaultValue(x => x.Timestamp);

            var embedsCount = embeds.Count;
            for (var i = 0; i < embedsCount; i++)
            {
                var embed = embeds[i];
                
                embed.Title = title;
                embed.Timestamp = timestamp;

                embed.Footer = new DiscordEmbedBuilder.EmbedFooter
                {
                    Text = $"Page {i + 1} of {embedsCount}"
                };
            }

            return embeds.Select(x => new Page(embed: x)).ToList();
        }
        
        public static async Task RespondWithPaginatedEmbedsAsync(this CommandContext context, List<DiscordEmbedBuilder> embeds)
            => await context.Channel.SendPaginatedMessageAsync(context.Member, embeds.Paginate());

    }
}