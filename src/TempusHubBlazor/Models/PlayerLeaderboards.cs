using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Models.Tempus.Responses;

namespace TempusHubBlazor.Models
{
    public class PlayerLeaderboards
    {
        public RanksOverviewModel Overall { get; set; }
        public RanksOverviewModel Soldier { get; set; }
        public RanksOverviewModel Demoman { get; set; }
    }
}
