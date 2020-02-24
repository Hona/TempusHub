using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Models.Tempus;
using TempusHubBlazor.Models.Tempus.Activity;

namespace TempusHubBlazor.Utilities
{
    public static class TempusDataTransformationExtensions
    {
        public static RecordModel ToRecordModel(this RecordInfoShort record) => new RecordModel
        {
            Duration = record.Duration,
            Id = record.Id,
            Name = record.Name,
            SteamId = record.PlayerInfo.SteamId
        };
    }
}
