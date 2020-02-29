using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TempusHubBlazor.Models.Tempus.Responses
{
    public class ServerDemoModel
    {
        [JsonProperty(PropertyName = "id")]
        public int DemoId { get; set; }
        [JsonProperty(PropertyName = "mapname")]
        public string MapName { get; set; }
        [JsonProperty(PropertyName = "filename")]
        public string FileName { get; set; }
        [JsonProperty(PropertyName = "date")]
        public double Date { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "recording")]
        public bool Recording { get; set; }
        [JsonProperty(PropertyName = "requested")]
        public bool Requested { get; set; }
        [JsonProperty(PropertyName = "uploader_id")]
        public int? UploaderId { get; set; }
        [JsonProperty(PropertyName = "server_id")]
        public int ServerId{ get; set; }
        [JsonProperty(PropertyName = "expired")]
        public bool Expired { get; set; }
        [JsonProperty(PropertyName = "deleted")]
        public bool Deleted { get; set; }
    }
}
