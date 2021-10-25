namespace TempusHub.Web.ViewModels;

public class ServerInfoShortResponse
{
    public string Alias { get; set; }
    public string Name { get; set; }
    public int CurrentPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public string CurrentMap { get; set; }
    public string IpAddress { get; set; }
    public int Id { get; set; }
}