using System;
using LogApi = StardewModdingAPI.Log;

namespace Igorious.StardewValley.DynamicAPI.Utils
{
    public static class Log
    {
        public static void Error(string message)
        {
            LogApi.SyncColour($"[NMM] {message}", ConsoleColor.Red);
        }

        public static void Fail(string message)
        {
            LogApi.SyncColour($"[NMM] {message}", ConsoleColor.DarkRed);
        }

        public static void Info(string message)
        {
            LogApi.SyncColour($"[NMM] {message}", ConsoleColor.DarkGray);
        }

        public static void ImportantInfo(string message)
        {
            LogApi.SyncColour($"[NMM] {message}", ConsoleColor.DarkYellow);
        }
    }
}
