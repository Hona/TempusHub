using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.Activity
{
    public class PlayerInfo
    {
        [JsonProperty(PropertyName = "steamid")]
        public string Steamid { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}