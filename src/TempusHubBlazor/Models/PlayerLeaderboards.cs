using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Models.Tempus.Responses;

namespace TempusHubBlazor.Models
{
    public class PlayerLeaderboards
    {
        public List<RanksOverviewModel> Overall { get; set; }
        public List<RanksOverviewModel> Soldier { get; set; }
        public List<RanksOverviewModel> Demoman { get; set; }
    }
}
