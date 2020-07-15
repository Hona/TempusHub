using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Models.Tempus;

namespace TempusHubBlazor.Models
{
    public class RankedServerPlayerModel
    {
        public ServerPlayerModel Player { get; set; }
        public int? Rank { get; set; }
        public int? Class { get; set; }
    }
}
