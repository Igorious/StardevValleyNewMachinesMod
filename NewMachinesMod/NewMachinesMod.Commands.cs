using System;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Services;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.NewMachinesMod
{
    partial class NewMachinesMod
    {
        private void RegisterCommands()
        {
            Command.RegisterCommand(
                "nmm_out_craftables",
                "Outputs a list of craftable items | nmm_out_craftables",
                new[] { "" })
                .CommandFired += ExecuteOutCraftables;

            Command.RegisterCommand(
                "nmm_player_addcraftable",
                "Gives the player craftable item with specified ID | nmm_player_addcraftable <id>",
                new[] { "(Int32)<id>" })
                .CommandFired += ExecutePlayerAddCraftable;

            Command.RegisterCommand(
                "nmm_machine_showinputs",
                "Show all possible input items for selected machine or machine with specified ID | nmm_machine_showinputs [id]",
                new[] { "(Int32)<id>" })
                .CommandFired += ExecuteMachineShowInputs;
        }

        private static void ExecuteOutCraftables(object sender, EventArgsCommand e)
        {
            foreach (var craftableID in Game1.bigCraftablesInformation.Keys)
            {
                try
                {
                    var craftable = new Object(Vector2.Zero, craftableID);
                    Log.SyncColour($"[ID={craftableID:D3}] {craftable.Name}", ConsoleColor.Gray);
                }
                catch { }              
            }           
        }

        private static void ExecuteMachineShowInputs(object sender, EventArgsCommand e)
        {
            int id;
            var farmer = Game1.player;

            var args = e.Command.CalledArgs;
            if (args.Length > 0)
            {
                if (!CheckArgumentIsCraftableID(args)) return;
                id = args[0].AsInt32();
            }
            else
            {
                var activeObject = farmer.ActiveObject;
                if (activeObject == null || !activeObject.bigCraftable)
                {
                    Log.Async("Current object is not machine");
                    return;
                }
                id = activeObject.ParentSheetIndex;
            }
                    
            var machine = new Object(Vector2.Zero, id);

            var classMapper = ClassMapperService.Instance;
            if (classMapper.CraftableTypeMap.ContainsKey(id) || classMapper.DynamicCraftableTypeMap.ContainsKey(id))
            {
                machine = classMapper.ToSmartObject(machine);
            }
            
            Log.SyncColour($"Inputs for {machine.Name} (ID={machine.ParentSheetIndex}):", ConsoleColor.Gray);
            var itemIDs = Game1.objectInformation.Keys.ToList();
            foreach (var itemID in itemIDs)
            {
                var item = new Object(itemID, 1);
                machine.performObjectDropInAction(item, true, farmer);
                var canDrop = (machine.heldObject != null);
                machine.heldObject = null;
                if (canDrop) Log.SyncColour($"[ID={itemID:D3}] {item.Name}", ConsoleColor.Gray);
            }
        }

        private static void ExecutePlayerAddCraftable(object sender, EventArgsCommand e)
        {
            var args = e.Command.CalledArgs;
            if (!CheckArgumentIsCraftableID(args)) return;

            var id = args[0].AsInt32();
            var o = new Object(Vector2.Zero, id);
            Game1.player.addItemByMenuIfNecessary(o);
        }

        private static bool CheckArgumentIsCraftableID(string[] args)
        {
            if (args.Length == 0)
            {
                Log.LogValueNotSpecified();
                return false;
            }

            if (!args[0].IsInt32())
            {
                Log.LogValueNotInt32();
                return false;
            }

            var id = args[0].AsInt32();
            if (!Game1.bigCraftablesInformation.ContainsKey(id))
            {
                Log.AsyncR("<id> is invalid");
                return false;
            }

            return true;
        }
    }
}
