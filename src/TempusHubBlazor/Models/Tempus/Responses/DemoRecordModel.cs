using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus.Responses
{
    public class DemoRecordModel
    {
        [JsonProperty(PropertyName = "demo_id")]
        public int DemoId { get; set; }
        [JsonProperty(PropertyName = "server_id")]
        public int ServerId { get; set; }
        [JsonProperty(PropertyName = "player_class")]
        public int PlayerClass { get; set; }
        [JsonProperty(PropertyName = "start_tick")]
        public long StartTick { get; set; }
        [JsonProperty(PropertyName = "end_tick")]
        public long EndTick { get; set; }
        [JsonProperty(PropertyName = "date")]
        public double Date { get; set; }
        [JsonProperty(PropertyName = "duration")]
        public double Duration { get; set; }
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }
    }
}
