using TempusApi.Models;
using TempusApi.Models.Activity;

namespace TempusHub.Core.Utilities;

public static class TempusDataTransformationExtensions
{
    public static RecordModel ToRecordModel(this RecordInfoShort record) => new()
    {
        Duration = record.Duration,
        Id = record.Id,
        Name = record.Name,
        SteamId = record.PlayerInfo.SteamId
    };
}