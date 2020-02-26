using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Models.Tempus;
using TempusHubBlazor.Models.Tempus.Activity;

namespace TempusHubBlazor.Utilities
{
    public static class RecordExtensions
    {
        public static RecordInfoShort[] OrderByDuration(this List<RecordInfoShort> records)
            => records.OrderBy(x => x.Duration).ToArray();
        public static RecordModel[] OrderByDuration(this List<RecordModel> records)
            => records.OrderBy(x => x.Duration).ToArray();
    }
}
