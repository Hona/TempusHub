namespace TempusHub.Web.ViewModels;

public class TopPlayerOnlineResponse
{
    public string SteamName { get; set; }
    public string RealName { get; set; }
    public ServerInfoShortResponse ServerInfo { get; set; }
    public int TempusId { get; set; }
    public int Rank { get; set; }
    public int RankClass { get; set; }
}