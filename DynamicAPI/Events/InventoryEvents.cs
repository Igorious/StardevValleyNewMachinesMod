using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Services;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Events
{
    public sealed class InventoryEvents
    {
        static InventoryEvents()
        {
            GameEvents.UpdateTick += ActiveObjectChangedHanler.OnUpdateTick;
            MenuEvents.MenuChanged += CraftedObjectChangedHandler.OnMenuChanged;
        }

        public static event Action<ObjectEventArgs> ActiveObjectChanged;
        public static event Action<ObjectEventArgs> CraftedObjectChanged;

        private static class ActiveObjectChangedHanler
        {
            private static Object PreviousActiveObject { get; set; }

            public static void OnUpdateTick(object sender, EventArgs e)
            {
                var activeObject = Game1.player.ActiveObject;
                if (activeObject == PreviousActiveObject) return;

                var args = new ObjectEventArgs(activeObject);
                ActiveObjectChanged?.Invoke(args);
                if (activeObject != args.Object)
                {
                    PreviousActiveObject = Game1.player.ActiveObject = args.Object;
                }
            }
        }

        private static class CraftedObjectChangedHandler
        {
            private static CraftingPage CraftingMenu { get; set; }
            private static Object PreviousCraftedObject { get; set; }

            public static void OnMenuChanged(object sender, EventArgsClickableMenuChanged e)
            {
                if (!(e.NewMenu is GameMenu)) return;

                CraftingMenu = e.NewMenu.GetField<List<IClickableMenu>>("pages").OfType<CraftingPage>().First();
                GameEvents.UpdateTick += OnCrafted;
                MenuEvents.MenuClosed += OnCraftingMenuClosed;
            }

            private static void OnCraftingMenuClosed(object sender, EventArgsClickableMenuClosed eventArgsClickableMenuClosed)
            {
                MenuEvents.MenuClosed -= OnCraftingMenuClosed;
                GameEvents.UpdateTick -= OnCrafted;
                CraftingMenu = null;
                PreviousCraftedObject = null;
            }

            private static void OnCrafted(object sender, EventArgs e)
            {
                var heldItem = CraftingMenu.GetField<Item>("heldItem") as Object;
                if (PreviousCraftedObject == heldItem) return;

                var args = new ObjectEventArgs(heldItem);
                CraftedObjectChanged?.Invoke(args);
                if (heldItem != args.Object)
                {
                    CraftingMenu.SetField<Item>("heldItem", PreviousCraftedObject = args.Object);
                }
            }
        }
    }
}