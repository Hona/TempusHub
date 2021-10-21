using TempusHubBlazor.Models.Tempus;

namespace TempusHubBlazor.Models;

public class RankedServerPlayerModel
{
    public ServerPlayerModel Player { get; set; }
    public int? Rank { get; set; }
    public int? Class { get; set; }
}