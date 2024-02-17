using TempusApi.Enums;

namespace TempusHub.Web.ViewModels;

public class TopPlayerOnlineResponse
{
    public string SteamName { get; set; }
    public string RealName { get; set; }
    public ServerInfoShortResponse ServerInfo { get; set; }
    public long TempusId { get; set; }
    public long Rank { get; set; }
    public Class RankClass { get; set; }
}