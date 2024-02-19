using TempusApi.Enums;
using TempusApi.Models;

namespace TempusHub.Core.Models;

public class RankedServerPlayerModel
{
    public ServerPlayerModel Player { get; set; }
    public long? Rank { get; set; }
    public Class? Class { get; set; }
}