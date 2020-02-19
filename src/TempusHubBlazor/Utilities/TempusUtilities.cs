using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Constants;
using TempusHubBlazor.Models.MySQL;
using TempusHubBlazor.Models.Tempus.Responses;

namespace TempusHubBlazor.Utilities
{
    /// <summary>
    /// Manipulates data provided from the tempus API for code usage
    /// </summary>
    public static class TempusUtilities
    {
        /// <summary>
        /// The time that the API uses as 0 seconds
        /// </summary>
        private static DateTime UnixEpoch = new DateTime(1970, 1, 1);
        /// <summary>
        /// Returns DateTime from tempus API seconds
        /// </summary>
        /// <param name="seconds">Tempus API seconds (from Unix Epoch)</param>
        /// <returns></returns>
        public static DateTime GetDateFromSeconds(double seconds) => UnixEpoch.AddSeconds(seconds);
        /// <summary>
        /// Returns the first character of the class name
        /// </summary>
        /// <param name="id">Class ID</param>
        /// <returns></returns>
        public static string GetClassChar(int id)
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
        /// <summary>
        /// Returns the world record split string using activity data, as well as map API times
        /// </summary>
        /// <param name="cache">The locally stored (MySQL) stored times</param>
        /// <param name="zonedResults">API request for the record</param>
        /// <returns>Formatted string showing the true WR time save</returns>
        public static string GetWRSplitString(MapRecordCache cache, ZonedRecordsModel zonedResults)
        {
            double slowRecord;
            // If the current wr doesn't have a value, it can be assumed there is no old duration
            if (cache == null || !cache.CurrentWRDuration.HasValue || !cache.OldWRDuration.HasValue)
            {
                slowRecord = cache.ClassId == 4 ? zonedResults.Runs.DemomanRuns.OrderBy(x => x.Duration).ToArray()[1].Duration 
                    : zonedResults.Runs.SoldierRuns.OrderBy(x => x.Duration).ToArray()[1].Duration;
            }
            else
            {
                slowRecord = cache.OldWRDuration.Value;
            }

            var timeSplit = slowRecord - cache.CurrentWRDuration.Value;
            return "WR -" + FormattedDuration(timeSplit);
        }
        /// <summary>
        /// Returns a nice string of the duration
        /// </summary>
        /// <param name="duration">Duration is in seconds</param>
        public static string FormattedDuration(double duration)
        {
            var seconds = (int)Math.Truncate(duration);
            var milliseconds = (duration - (int)Math.Truncate(duration)) * 1000;
            var timespan = new TimeSpan(0, 0, 0, seconds, (int)Math.Truncate(milliseconds));
            return timespan.Days > 0 ? timespan.ToString(@"dd\:hh\:mm\:ss\.fff") : timespan.ToString(timespan.Hours > 0 ? @"hh\:mm\:ss\.fff" : @"mm\:ss\.fff");
        }
        /// <summary>
        /// Returns a formatted string of the tick duration
        /// </summary>
        /// <param name="ticks">TF2 game ticks</param>
        public static string TicksToFormattedTime(long ticks)
        {
            var timeSpan = TicksToTimeSpan(ticks);
            return FormattedDuration(timeSpan.TotalSeconds);
        }
        /// <summary>
        /// Converts TF2 game ticks into a TimeSpan
        /// </summary>
        /// <param name="ticks">TF2 game ticks</param>
        public static TimeSpan TicksToTimeSpan(long ticks) => new TimeSpan(ticks * 149998);
        public static string GetYoutubeUrl(string id)
        {
            if (id == null)
            {
                return null;
            }

            return "https://youtubehub.com/watch?v=" + id;
        }
    }
}
