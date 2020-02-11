using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TempusHubBlazor.Models.MySQL
{
    public class MapRecordCache
    {
        public int MapId { get; set; }
        public double? CurrentWRDuration { get; set; }
        public double? OldWRDuration { get; set; }
    }
}
