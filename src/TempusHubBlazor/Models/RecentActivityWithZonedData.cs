using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TempusHubBlazor.Models
{
    public class RecentActivityWithZonedData
    {
        public RecentActivityWithZonedData()
        {
            MapWR = new List<RecordWithZonedData>();
            CourseWR = new List<RecordWithZonedData>();
            BonusWR = new List<RecordWithZonedData>();
            MapTT = new List<RecordWithZonedData>();
        }
        public List<RecordWithZonedData> MapWR { get; set; }
        public List<RecordWithZonedData> CourseWR { get; set; }

        public List<RecordWithZonedData> BonusWR { get; set; }

        public List<RecordWithZonedData> MapTT { get; set; }
    }
}
