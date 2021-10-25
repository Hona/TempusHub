using TempusApi.Models.Responses;

namespace TempusHub.Core.Models;

public class TopPlayerOnline
{
    public int Rank { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public string SteamId { get; set; }
    public ServerStatusModel Server { get; set; }
    public int RankClass { get; set; }
}