using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TempusHubBlazor.Models
{
    public class RecentActivityWithZonedData
    {
        public List<RecordWithZonedData> MapWR { get; set; }
        public List<RecordWithZonedData> CourseWR { get; set; }

        public List<RecordWithZonedData> BonusWR { get; set; }

        public List<RecordWithZonedData> MapTT { get; set; }
    }
}
