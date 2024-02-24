using System;
using Empedo.Discord.Models;
using TempusApi.Enums;

namespace Empedo.Discord.Helpers
{
    public static class TempusHelper
    {
        private const string TempusUrl = "https://tempus2.xyz";

        public static string GetEmote(this Class id) => id switch
        {
            Class.Soldier => "<:soldier:702841577806233702>",
            Class.Demoman => "<:demoman:702841578259087380>",
            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
        };
        
        public static string TicksToFormattedTime(long ticks)
        {
            var timeSpan = TicksToTimeSpan(ticks);
            return TimeSpanToFormattedTime(timeSpan);
        }

        public static string TimeSpanToFormattedTime(TimeSpan timeSpan)
        {
            var factor = (int) Math.Pow(10, 5);
            var roundedTimeSpan = new TimeSpan((long) Math.Round(1.0 * timeSpan.Ticks / factor) * factor);
            return
                $"{roundedTimeSpan.Days}:{roundedTimeSpan.Hours}:{roundedTimeSpan.Minutes}:{roundedTimeSpan.Seconds}.{Math.Round((double) roundedTimeSpan.Milliseconds)}"
                    .Trim('0', ':', '.');
        }

        public static string GetSteamId64(string steamId)
        {
            ulong output = 76561197960265728;

            var idParts = steamId.Split(":");

            output += ulong.Parse(idParts[2]) * 2;

            if (idParts[1] == "1")
            {
                output += 1;
            }
            
            return output.ToString();
        }
        
        public static DateTimeOffset GetDateFromTimestamp(double timestamp)
        {
            // Convert the timestamp to milliseconds and then to long
            var milliseconds = (long)(timestamp * 1000);

            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        }

        public static TimeSpan TicksToTimeSpan(long ticks) => new(ticks * 149998);
        public static Uri GetMapUrl(string name) => new($"{TempusUrl}/maps/" + name);
        public static Uri GetRecordUrl(long id) => new($"{TempusUrl}/records/" + id);
        public static Uri GetPlayerUrl(long id) => new($"{TempusUrl}/players/" + id);
        public static Uri GetDemoUrl(long id) => new($"{TempusUrl}/demos/" + id);
        public static Uri GetServerUrl(long id) => new($"{TempusUrl}/servers/" + id);
        public static Uri GetYoutubeUrl(string id) => new Uri("https://youtube.com/watch?v=" + id);
        
        public static string GetMapImageUrl(string mapName) =>
            $"https://static.tempus2.xyz/web/screenshots/raw/{mapName}_1080p.jpeg";
    }
}