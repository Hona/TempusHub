using TempusApi.Enums;
using TempusApi.Models.Responses;
using TempusHub.Core.Models;

namespace TempusHub.Core.Utilities;

/// <summary>
/// Manipulates data provided from the tempus API for code usage
/// </summary>
public static class TempusUtilities
{
    /// <summary>
    /// Returns DateTime from tempus API seconds
    /// </summary>
    /// <param name="seconds">Tempus API seconds (from Unix Epoch)</param>
    /// <returns></returns>
    public static DateTime GetDateFromSeconds(double seconds) => DateTime.UnixEpoch.AddSeconds(seconds);
    /// <summary>
    /// Returns the first character of the class name
    /// </summary>
    /// <param name="id">Class ID</param>
    /// <returns></returns>
    public static string GetClassChar(int id) => id switch
    {
        4 => "D",
        3 => "S",
        _ => id.ToString(),
    };
    /// <summary>
    /// Returns the world record split string using activity data, as well as map API times
    /// </summary>
    /// <param name="cache">The locally stored (MySQL) stored times</param>
    /// <param name="zonedResults">API request for the record</param>
    /// <returns>Formatted string showing the true WR time save</returns>
    public static string GetWrSplitString(MapRecordCache? cache, ZonedRecordsModel zonedResults)
    {
        double slowRecord;
        // If the current wr doesn't have a value, it can be assumed there is no old duration
        if (cache?.CurrentWrDuration == null || !cache.OldWrDuration.HasValue || 
            TimesEqual(cache.CurrentWrDuration.Value, cache.OldWrDuration.Value))
        {
            // Get #2 time
            var runs = cache.ClassId == Class.Demoman ? zonedResults.Runs.DemomanRuns 
                : zonedResults.Runs.SoldierRuns;

            if (runs.Count == 1)
            {
                // Only one run, split is N/A
                return "N/A";
            }

            slowRecord = runs.OrderByDuration()[1].Duration;
        }
        else
        {
            slowRecord = cache.OldWrDuration.Value;
        }

        var timeSplit = slowRecord - cache.CurrentWrDuration.Value;

        var symbol = TimesEqual(0, timeSplit) ? "-" : timeSplit < 0 ? "+" : "-";


        return "WR " + symbol + FormattedDuration(timeSplit);
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
    public static TimeSpan TicksToTimeSpan(long ticks) => new(ticks * 149998);
    public static string GetYoutubeUrl(string id)
    {
        if (id == null)
        {
            return null;
        }

        return "https://youtube.com/watch?v=" + id;
    }

    /// <summary>
    /// Simplifies the accuracy comparison of runs, when they are given in difference dp (12 vs 14)
    /// </summary>
    public static bool TimesEqual(double time1, double time2)
        // Allows tolerance past 12 dp
        => Math.Abs(time1 - time2) < 0.000000000001;

    public static bool FasterThan(this double time1, double time2)
    {
        if (TimesEqual(time1, time2))
        {
            return false;
        }

        return time1 < time2;
    }

    public static bool SlowerThan(this double time1, double time2)
    {
        if (TimesEqual(time1, time2))
        {
            return false;
        }

        return time1 > time2;
    }
}