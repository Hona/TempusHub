using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.Activity
{
    public class RecordInfoShort
    {
        [JsonProperty(PropertyName = "server_id")]
        public int ServerId { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "zone_id")]
        public int ZoneId { get; set; }

        [JsonProperty(PropertyName = "class")]
        public int Class { get; set; }

        [JsonProperty(PropertyName = "date")]
        public double Date { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public double Duration { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }
}