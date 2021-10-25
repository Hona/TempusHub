using TempusApi.Models;
using TempusApi.Models.Activity;

namespace TempusHub.Core.Utilities;

public static class RecordExtensions
{
    public static RecordInfoShort[] OrderByDuration(this List<RecordInfoShort> records)
        => records.OrderBy(x => x.Duration).ToArray();
    public static RecordModel[] OrderByDuration(this List<RecordModel> records)
        => records.OrderBy(x => x.Duration).ToArray();
}