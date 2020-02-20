using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TempusHubBlazor.Utilities
{
    public static class HrefHelper
    {
        public static string GetMapInfoPath(string mapName) => "/maps/" + mapName;
    }
}
