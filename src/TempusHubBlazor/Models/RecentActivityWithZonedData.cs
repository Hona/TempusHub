using System.Collections.Concurrent;
using System.Collections.Generic;

namespace TempusHubBlazor.Models;

public class RecentActivityWithZonedData
{
    public RecentActivityWithZonedData()
    {
        MapWr = new ConcurrentBag<RecordWithZonedData>();
        CourseWr = new ConcurrentBag<RecordWithZonedData>();
        BonusWr = new ConcurrentBag<RecordWithZonedData>();
        MapTt = new ConcurrentBag<RecordWithZonedData>();
    }
    public ConcurrentBag<RecordWithZonedData> MapWr { get; set; }
    public ConcurrentBag<RecordWithZonedData> CourseWr { get; set; }

    public ConcurrentBag<RecordWithZonedData> BonusWr { get; set; }

    public ConcurrentBag<RecordWithZonedData> MapTt { get; set; }
}