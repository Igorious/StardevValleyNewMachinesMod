using System;
using LogApi = StardewModdingAPI.Log;

namespace Igorious.StardewValley.DynamicAPI.Utils
{
    public static class Log
    {
        public static void Error(string message)
        {
            LogApi.SyncColour($"[NNM] {message}", ConsoleColor.Red);
        }

        public static void Info(string message)
        {
            LogApi.SyncColour($"[NNM] {message}", ConsoleColor.DarkGray);
        }

        public static void ImportantInfo(string message)
        {
            LogApi.SyncColour($"[NNM] {message}", ConsoleColor.DarkYellow);
        }
    }
}
