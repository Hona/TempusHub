﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace TempusHubBlazor.Models.Tempus.Responses;

public class ServerDemoFullOverview
{
    [JsonProperty(PropertyName = "server_info")]
    public ServerInfo ServerInfo { get; set; }
    [JsonProperty(PropertyName = "demo_info")]
    public ServerDemoModel Overview { get; set; }
    [JsonProperty(PropertyName = "demo_runs")]
    public List<DemoRunModel> Runs { get; set; }
}