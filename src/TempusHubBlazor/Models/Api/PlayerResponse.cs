using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TempusHubBlazor.Models.Tempus.Activity;

namespace TempusHubBlazor.Models.Api
{
    public class PlayerResponse : PlayerInfo
    {
        public string RealName { get; set; }
    }
}
