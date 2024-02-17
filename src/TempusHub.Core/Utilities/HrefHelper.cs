using System.Globalization;

namespace TempusHub.Core.Utilities;

public static class HrefHelper
{
    public static string GetMapInfoPath(string mapName) => "/map/" + mapName;
    public static string GetPlayerInfoPath(long playerId) => GetPlayerInfoPath(playerId.ToString(CultureInfo.InvariantCulture));
    public static string GetPlayerInfoPath(string playerId) => "/player/" + playerId;
    public static string GetServerInfoPath(string server) => "/server/" + server;
    public static string GetDemoInfoPath(long demoId) => "/demo/" + demoId;
    public static string GetRunInfoPath(long runId) => "/run/" + runId;

}