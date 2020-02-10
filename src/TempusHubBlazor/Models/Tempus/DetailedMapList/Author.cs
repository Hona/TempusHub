using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.DetailedMapList
{
    public class Author
    {
        [JsonProperty(PropertyName = "map_id")]
        public int MapId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }
}