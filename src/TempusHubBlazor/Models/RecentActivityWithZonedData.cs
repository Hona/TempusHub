using System.Collections.Generic;

namespace TempusHubBlazor.Models;

public class RecentActivityWithZonedData
{
    public RecentActivityWithZonedData()
    {
        MapWr = new List<RecordWithZonedData>();
        CourseWr = new List<RecordWithZonedData>();
        BonusWr = new List<RecordWithZonedData>();
        MapTt = new List<RecordWithZonedData>();
    }
    public List<RecordWithZonedData> MapWr { get; set; }
    public List<RecordWithZonedData> CourseWr { get; set; }

    public List<RecordWithZonedData> BonusWr { get; set; }

    public List<RecordWithZonedData> MapTt { get; set; }
}