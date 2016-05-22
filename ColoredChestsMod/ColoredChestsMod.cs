using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Services;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;

namespace Igorious.StardewValley.ColoredChestsMod
{
    public sealed class ColoredChestsMod : Mod
    {
        public static string RootPath { get; private set; }

        public override void Entry(params object[] objects)
        {
            RootPath = PathOnDisk;
            ClassMapperService.Instance.MapCraftable<ColoredChest>(CraftableID.Chest);
            RegisterCommands();
        }

        private static void RegisterCommands()
        {
            Command.RegisterCommand(
                "ccm_show",
                "Show all chests in current location. | ccm_show",
                new[] { "" })
                .CommandFired += ShowChests;

            Command.RegisterCommand(
                "ccm_set",
                "Set color to chest in specified location. | ccm_set <x> <y> <color>",
                new[] { "(Int32)<x> (Int32)<y> (String)<color>" })
                .CommandFired += SetColor;
        }

        private static void ShowChests(object sender, EventArgsCommand e)
        {
            var chests = Game1.currentLocation.Objects
                .Where(o => o.Value is Chest && o.Value.bigCraftable && o.Value.ParentSheetIndex == (int)CraftableID.Chest)
                .OrderBy(o => o.Key.Y)
                .ThenBy(o => o.Key.X)
                .ToList();

            if (!chests.Any())
            {
                Log.Info("There's no chests in current location.");
                return;
            }

            var playerPosition = Game1.player.Position / Game1.tileSize;
            Log.SyncColour($"Player: X={playerPosition.X:F1}, Y={playerPosition.Y:F1}", ConsoleColor.Cyan);
            foreach (var p in chests)
            {
                var location = p.Key;
                Log.SyncColour($"Chest: X={location.X}, Y={location.Y}", ConsoleColor.Cyan);
            }
        }

        private static void SetColor(object sender, EventArgsCommand e)
        {
            var args = e.Command.CalledArgs;
            if (args.Length < 3)
            {
                Log.Error("Wrong arguments count.");
                return;
            }

            if (!args[0].IsInt32())
            {
                Log.Error("First argument must be int.");
                return;
            }
            var x = args[0].AsInt32();

            if (!args[1].IsInt32())
            {
                Log.Error("Second argument must be int.");
                return;
            }
            var y = args[1].AsInt32();

            var colorName = args[2];
            Color color;
            if (Regex.Match(colorName, @"#?[\dA-Fa-f]{6}").Success)
            {
                color = RawColor.FromHex(colorName.Substring(1)).ToXnaColor();
            }
            else
            {
                var propertyInfo = typeof(Color).GetProperty(colorName, BindingFlags.Static | BindingFlags.Public);
                if (propertyInfo == null)
                {
                    Log.Error("Third argument must be color name.");
                    return;
                }
                color = (Color)propertyInfo.GetValue(null);
            }

            var chest = Game1.currentLocation.Objects
                .FirstOrDefault(p => p.Key == new Vector2(x, y) && p.Value is Chest)
                .Value as Chest;
            if (chest == null)
            {
                Log.Error("Chest is not found.");
                return;
            }

            chest.tint = color;
        }
    }
}
