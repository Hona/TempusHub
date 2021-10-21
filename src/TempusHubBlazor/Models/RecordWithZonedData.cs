﻿using TempusHubBlazor.Models.Tempus.Activity;
using TempusHubBlazor.Models.Tempus.Responses;

namespace TempusHubBlazor.Models;

public class RecordWithZonedData
{
    public TempusRecordBase Record { get; set; }
    public ZonedRecordsModel ZonedData { get; set; }
}