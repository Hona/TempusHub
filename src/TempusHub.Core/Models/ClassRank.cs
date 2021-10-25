using TempusApi.Models;

namespace TempusHub.Core.Models;

public class ClassRank
{
    public int Class { get; set; }
    public int Rank { get; set; }
    public ServerPlayerModel Player { get; set; }
}