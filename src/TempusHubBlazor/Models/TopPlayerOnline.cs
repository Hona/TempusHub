using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Models.Tempus;
using TempusHubBlazor.Models.Tempus.Responses;

namespace TempusHubBlazor.Models
{
    public class TopPlayerOnline
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public string SteamId { get; set; }
        public ServerStatusModel Server { get; set; }
    }
}
