using System.Linq;
using Igorious.StardewValley.DynamicAPI.Utils;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Objects;

namespace Igorious.StardewValley.DynamicAPI.Services.Internal
{
    public sealed class Traverser
    {
        #region Singleton Access

        private Traverser() {}

        private static Traverser _instance;

        public static Traverser Instance => _instance ?? (_instance = new Traverser());

        #endregion

        #region	Public Methods

        public static void ConvertObjectsInLocation(GameLocation location, System.Predicate<Object> condition, System.Func<Object, Object> convert)
        {
            var locationObjects = location.Objects;
            var wrongObjectInfos = locationObjects.Where(o => condition(o.Value)).ToList();
            if (wrongObjectInfos.Count != 0)
            {
                Log.Info($"Found {wrongObjectInfos.Count} objects in {location.Name}.");
                foreach (var wrongObjectInfo in wrongObjectInfos)
                {
                    locationObjects.Remove(wrongObjectInfo.Key);
                    var newObject = convert(wrongObjectInfo.Value);
                    locationObjects.Add(wrongObjectInfo.Key, newObject);
                }
            }

            ConvertObjectsInChests(location, condition, convert);
            ConvertObjectsInMachines(location, condition, convert);
            ConvertObjectsInBuildings(location, condition, convert);
        }

        public static void ConvertObjectsInInventory(Farmer farmer, System.Predicate<Object> condition, System.Func<Object, Object> convert)
        {
            var count = 0;
            for (var i = 0; i < farmer.Items.Count; ++i)
            {
                var chestObject = farmer.Items[i] as Object;
                if (chestObject == null || !condition(chestObject)) continue;
                farmer.Items[i] = convert(chestObject);
                ++count;
            }

            if (count != 0) Log.Info($"Found {count} objects in inventory.");
        }

        #endregion

        #region	Auxiliary Methods

        private static void ConvertObjectsInChests(GameLocation location, System.Predicate<Object> condition, System.Func<Object, Object> convert)
        {
            var chests = location.Objects.Values.OfType<Chest>().ToList();
            if (location is FarmHouse) chests.Add(((FarmHouse)location).fridge);
            foreach (var chest in chests)
            {
                var count = 0;
                for (var i = 0; i < chest.items.Count; ++i)
                {
                    var chestObject = chest.items[i] as Object;
                    if (chestObject == null || !condition(chestObject)) continue;
                    chest.items[i] = convert(chestObject);
                    ++count;
                }
                if (count != 0) Log.Info($"Found {count} objects in {chest.Name}.");
            }
        }

        private static void ConvertObjectsInMachines(GameLocation location, System.Predicate<Object> condition, System.Func<Object, Object> convert)
        {
            var count = 0;
            var machines = location.Objects.Values.Where(o => o.bigCraftable).ToList();
            foreach (var machine in machines)
            {
                var heldObject = machine.heldObject;
                if (heldObject == null || !condition(heldObject)) continue;
                machine.heldObject = convert(heldObject);
                ++count;
            }
            if (count != 0) Log.Info($"Found {count} objects in machines.");
        }

        private static void ConvertObjectsInBuildings(GameLocation location, System.Predicate<Object> condition, System.Func<Object, Object> convert)
        {
            var buildableLocation = location as BuildableGameLocation;
            if (buildableLocation == null) return;

            foreach (var building in buildableLocation.buildings)
            {
                if (building.indoors != null)
                {
                    ConvertObjectsInLocation(building.indoors, condition, convert);
                }
            }
        }

        #endregion
    }
}