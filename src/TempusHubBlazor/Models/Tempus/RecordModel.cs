using System;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;

namespace TempusHubBlazor.Models.Tempus
{
    public class RecordModel
    {


        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "steamid")]
        public string SteamId { get; set; }

        [JsonProperty(PropertyName = "duration")]
        public double Duration { get; set; }
    }
}