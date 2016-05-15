using System;
using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace Igorious.StardewValley.DynamicAPI.Events
{
    public static class SavingEvents
    {
        public static event Action BeforeSaving;
        public static event Action AfterSaving;

        static SavingEvents()
        {
            MenuEvents.MenuClosed += OnMenuClosed;
            TimeEvents.OnNewDay += OnNewDay;
        }

        private static void OnNewDay(object sender, EventArgsNewDay e)
        {
            if (e.IsNewDay) return;
            BeforeSaving?.Invoke();
        }

        private static void OnMenuClosed(object sender, EventArgsClickableMenuClosed args)
        {
            if (!(args.PriorMenu is SaveGameMenu)) return;
            AfterSaving?.Invoke();
        }
    }
}