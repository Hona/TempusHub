using System;
using System.Collections.Generic;
using DSharpPlus.Entities;

namespace Empedo.Discord.Models
{
    public class TempusEmbedsCache
    {
        public List<DiscordEmbedBuilder> Embeds { get; set; }
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public TempusEmbedsCache(List<DiscordEmbedBuilder> embeds)
        {
            Embeds = embeds;
        }
    }
}