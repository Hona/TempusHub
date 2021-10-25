using TempusApi.Models.Activity;
using TempusApi.Models.Responses;

namespace TempusHub.Core.Models;

public class RecordWithZonedData
{
    public TempusRecordBase Record { get; set; }
    public ZonedRecordsModel ZonedData { get; set; }
}