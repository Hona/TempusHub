using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TempusHubBlazor.Utilities
{
    public static class HrefHelper
    {
        public static string GetMapInfoPath(string mapName) => "/map/" + mapName;
        public static string GetPlayerInfoPath(string playerId) => "/player/" + playerId;
        public static string GetServerInfoPath(string server) => "/server/" + server;
    }
}
