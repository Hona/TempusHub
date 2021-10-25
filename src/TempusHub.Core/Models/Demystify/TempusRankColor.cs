using Newtonsoft.Json;

namespace TempusHub.Core.Models.Demystify;

public class TempusRankColor
{
    [JsonProperty(PropertyName = "title")]
    public string Title { get; set; }
    [JsonProperty(PropertyName = "brackets_color")]
    public string BracketsColor { get; set; }
    [JsonProperty(PropertyName = "title_color")]
    public string TitleColor { get; set; }
    [JsonProperty(PropertyName = "name_color")]
    public string NameColor { get; set; }
    [JsonProperty(PropertyName = "chat_color")]
    public string ChatColor { get; set; }
}