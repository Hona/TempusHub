using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.Activity
{
    public class RecordInfo
    {
        [JsonProperty(PropertyName = "demo_id")]
        public int DemoId { get; set; }

        [JsonProperty(PropertyName = "server_id")]
        public int ServerId { get; set; }

        [JsonProperty(PropertyName = "user_id")]
        public int UserId { get; set; }

        [JsonProperty(PropertyName = "zone_id")]
        public int ZoneId { get; set; }

        [JsonProperty(PropertyName = "demo_start_tick")]
        public int DemoStartTick { get; set; }

        [JsonProperty(PropertyName = "rank")]
        public int Rank { get; set; }

        [JsonProperty(PropertyName = "class")]
        public int Class { get; set; }

        [JsonProperty(PropertyName = "date")]
        public double Date { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public double Duration { get; set; }

        [JsonProperty(PropertyName = "demo_end_tick")]
        public int DemoEndTick { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }
}