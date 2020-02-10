using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Constants;

namespace TempusHubBlazor.Utilities
{
    public static class TempusUtilities
    {
        public static string GetClass(int id)
        {
            switch (id)
            {
                case 4:
                    return "D";
                case 3:
                    return "S";
                default:
                    return id.ToString();
            }
        }

        public static string TicksToFormattedTime(long ticks)
        {
            var timeSpan = TicksToTimeSpan(ticks);
            return TimeSpanToFormattedTime(timeSpan);
        }

        public static string TimeSpanToFormattedTime(TimeSpan timeSpan)
        {
            var factor = (int)Math.Pow(10, 7 - TempusDataConstants.RoundingSize);
            var roundedTimeSpan = new TimeSpan((long)Math.Round(1.0 * timeSpan.Ticks / factor) * factor);
            return
                $"{roundedTimeSpan.Days}:{roundedTimeSpan.Hours}:{roundedTimeSpan.Minutes}:{roundedTimeSpan.Seconds}.{Math.Round((double)roundedTimeSpan.Milliseconds)}"
                    .Trim('0', ':', '.');
        }

        public static TimeSpan TicksToTimeSpan(long ticks) => new TimeSpan(ticks * 149998);
        public static string GetMapUrl(string name) => "https://tempus.xyz/maps/" + name;
        public static string GetRecordUrl(int id) => "https://tempus.xyz/records/" + id;
        public static string GetPlayerUrl(int id) => "https://tempus.xyz/players/" + id;
        public static string GetDemoUrl(int id) => "https://tempus.xyz/demos/" + id;
        public static string GetServerUrl(int id) => "https://tempus.xyz/servers/" + id;
        public static string GetYoutubeUrl(string id) => "https://youtube.com/watch?v=" + id;
    }
}
