using TempusApi.Enums;
using TempusApi.Models;

namespace TempusHub.Core.Models;

public class ClassRank
{
    public Class Class { get; set; }
    public long Rank { get; set; }
    public ServerPlayerModel Player { get; set; }
}