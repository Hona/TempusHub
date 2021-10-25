using TempusApi.Models.Responses;

namespace TempusHub.Core.Models;

public class PlayerLeaderboards
{
    public RanksOverviewModel Overall { get; set; }
    public RanksOverviewModel Soldier { get; set; }
    public RanksOverviewModel Demoman { get; set; }
}