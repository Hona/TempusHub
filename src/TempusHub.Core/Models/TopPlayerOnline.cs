using TempusApi.Enums;
using TempusApi.Models.Responses;

namespace TempusHub.Core.Models;

public class TopPlayerOnline
{
    public long Rank { get; set; }
    public long Id { get; set; }
    public string Name { get; set; }
    public string SteamId { get; set; }
    public ServerStatusModel Server { get; set; }
    public Class RankClass { get; set; }
}