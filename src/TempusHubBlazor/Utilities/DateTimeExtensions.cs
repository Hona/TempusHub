using System;
using System.Text;

namespace TempusHubBlazor.Utilities
{
    public static class DateTimeExtensions
    {
        public static string GetTimeStringSinceDateTime(this DateTime dateTime)
        {
            TimeSpan deltaTime;
            if (dateTime.Ticks < DateTime.Now.Ticks)
            {
                deltaTime = DateTime.Now - dateTime;
                return deltaTime.ToPrettyFormat() + " ago";
            }
            else
            {
                deltaTime = dateTime - DateTime.Now;
                return deltaTime.ToPrettyFormat() + " in the future";

            }
        }
        public static string ToPrettyFormat(this TimeSpan span)
        {

            if (span == TimeSpan.Zero) return "0 minutes";

            var sb = new StringBuilder();
            if (span.Days > 0)
                return sb.AppendFormat("{0} day{1} ", span.Days, span.Days > 1 ? "s" : string.Empty).ToString();
            if (span.Hours > 0)
                return sb.AppendFormat("{0} hour{1} ", span.Hours, span.Hours > 1 ? "s" : string.Empty).ToString();
            if (span.Minutes > 0)
                return sb.AppendFormat("{0} minute{1} ", span.Minutes, span.Minutes > 1 ? "s" : string.Empty).ToString();
            return sb.ToString();

        }
    }
}
