using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class ClassMapperService
    {
        #region Singleton Access

        private ClassMapperService()
        {
            LocationEvents.LocationObjectsChanged += OnLocationObjectsChanged;
            TimeEvents.OnNewDay += (s, e) =>
            {
                if (e.IsNewDay) return;

                LocationEvents.LocationObjectsChanged -= OnLocationObjectsChanged;
                Log.SyncColour("Deactivating objects (before saving)...", ConsoleColor.DarkGray);
                CovertToRawObjects(); // Allow use native serializer.
                MenuEvents.MenuClosed += OnMenuClosed; // Prevent issue with crash after finishing bundle.
            };
        }

        private static ClassMapperService _instance;

        public static ClassMapperService Instance => _instance ?? (_instance = new ClassMapperService());

        #endregion

        #region Private Data

        private readonly Dictionary<int, Type> _itemTypeMap = new Dictionary<int, Type>();
        private readonly Dictionary<int, Type> _craftableTypeMap = new Dictionary<int, Type>();
        private readonly Dictionary<int, DynamicTypeInfo> _dynamicCraftableTypeMap = new Dictionary<int, DynamicTypeInfo>();

        #endregion

        #region	Public Methods

        /// <summary>
        /// Map game Item ID to specific class.
        /// </summary>
        public void MapItem<TObject>(int id) where TObject : ISmartObject, new()
        {
            _itemTypeMap.Add(id, typeof(TObject));
        }

        /// <summary>
        /// Map game Craftable ID to specific class.
        /// </summary>
        public void MapCraftable<TObject>(int id) where TObject : ISmartObject, new()
        {
            _craftableTypeMap.Add(id, typeof(TObject));
        }

        /// <summary>
        /// Map game Craftable ID to specific class.
        /// </summary>
        public void MapCraftable(DynamicTypeInfo dynamicTypeInfo)
        {
            _dynamicCraftableTypeMap.Add(dynamicTypeInfo.ClassID, dynamicTypeInfo);
        }

        /// <summary>
        /// Get mapped Craftable ID for specific class.
        /// </summary>
        public int GetCraftableID<TObject>() where TObject : ISmartObject
        {
            return _craftableTypeMap.First(kv => kv.Value == typeof(TObject)).Key;
        }

        /// <summary>
        /// Get mapped ID for specific class.
        /// </summary>
        public int GetItemID<TObject>() where TObject : ISmartObject
        {
            return _itemTypeMap.First(kv => kv.Value == typeof(TObject)).Key;
        }

        #endregion

        #region	Auxiliary Methods

        private void OnMenuClosed(object sender, EventArgsClickableMenuClosed args)
        {
            if (!(args.PriorMenu is SaveGameMenu)) return;

            MenuEvents.MenuClosed -= OnMenuClosed;
            Log.SyncColour("Activating objects (after saving)...", ConsoleColor.DarkGray);
            CovertToSmartObjects(); // Activate objects in farmer house.
            LocationEvents.LocationObjectsChanged += OnLocationObjectsChanged;
        }

        private void OnLocationObjectsChanged(object sender, EventArgsLocationObjectsChanged eventArgsLocationObjectsChanged)
        {
            Log.SyncColour($"Activating objects (changes in {Game1.currentLocation?.Name})...", ConsoleColor.DarkGray);
            CovertToSmartObjects();
        }

        private Object ToSmartObject(Object rawObject)
        {
            Type type;
            Object smartObject;
            if (rawObject.bigCraftable)
            {
                if (_craftableTypeMap.TryGetValue(rawObject.ParentSheetIndex, out type))
                {
                    smartObject = (Object)Activator.CreateInstance(type);
                    Log.SyncColour($"Wrapped obj {rawObject.Name} into {smartObject.GetType().Name}", ConsoleColor.DarkGray);
                }
                else
                {
                    var dynamicType = _dynamicCraftableTypeMap[rawObject.ParentSheetIndex];
                    var ctor = dynamicType.BaseType.GetConstructor(new[] { typeof(int) });
                    smartObject = (Object)ctor.Invoke(new object[] { dynamicType.ClassID });
                }
            }
            else
            {
                type = _itemTypeMap[rawObject.ParentSheetIndex];
                smartObject = (Object)Activator.CreateInstance(type);
                Log.SyncColour($"Wrapped obj {rawObject.Name} into {smartObject.GetType().Name}", ConsoleColor.DarkGray);
            }

            CopyProperties(rawObject, smartObject);
            return smartObject;
        }

        private Object ToRawObject(Object smartObject)
        {
            if (smartObject is ColoredObject)
            {
                var rawObject = new ColoredObject(smartObject.ParentSheetIndex, smartObject.stack, Color.White);
                CopyProperties(smartObject, rawObject);
                return rawObject;
            }
            else
            {
                var type = smartObject.bigCraftable? ObjectFactory.bigCraftable : ObjectFactory.regularObject;                
                var rawObject = (Object)ObjectFactory.getItemFromDescription(type, smartObject.ParentSheetIndex, 1);
                CopyProperties(smartObject, rawObject);
                return rawObject;
            }
        }

        private static void CopyProperties(Object from, Object to)
        {
            var properties = typeof(Object).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite);
            foreach (var property in properties)
            {
                property.SetValue(to, property.GetValue(from));
            }

            var fields = typeof(Object).GetFields(BindingFlags.Instance | BindingFlags.Public).Where(f => !f.IsInitOnly);
            foreach (var field in fields)
            {
                field.SetValue(to, field.GetValue(from));
            }

            if (from is ColoredObject && to is ColoredObject)
            {
                ((ColoredObject)to).color = ((ColoredObject)from).color;
            }
        }

        private void CovertToSmartObjects()
        {
            var sw = Stopwatch.StartNew();
            ConvertObjectsInLocation(Game1.currentLocation, IsRawObject, ToSmartObject);
            Game1.getAllFarmers().ForEach(f => ConvertObjectsInInventory(f, IsRawObject, ToSmartObject));
            Log.SyncColour($"Convertion ('smart') finished: {sw.ElapsedMilliseconds} ms", ConsoleColor.DarkGray);
        }

        private void CovertToRawObjects()
        {
            var sw = Stopwatch.StartNew();
            foreach (var gameLocation in Game1.locations)
            {
                ConvertObjectsInLocation(gameLocation, IsSmartObject, ToRawObject);
            }
            Game1.getAllFarmers().ForEach(f => ConvertObjectsInInventory(f, IsSmartObject, ToRawObject));
            Log.SyncColour($"Convertion ('raw') finished: {sw.ElapsedMilliseconds} ms", ConsoleColor.DarkGray);
        }

        private bool IsRawObject(Object o)
        {
            return _craftableTypeMap.ContainsKey(o.ParentSheetIndex) && (o.GetType() != _craftableTypeMap[o.ParentSheetIndex])
                || _itemTypeMap.ContainsKey(o.ParentSheetIndex) && (o.GetType() != _itemTypeMap[o.ParentSheetIndex])
                || _dynamicCraftableTypeMap.ContainsKey(o.ParentSheetIndex) && !(o is IDynamic);
        }

        private bool IsSmartObject(Object o)
        {
            return (_craftableTypeMap.ContainsKey(o.ParentSheetIndex) || _itemTypeMap.ContainsKey(o.ParentSheetIndex) || _dynamicCraftableTypeMap.ContainsKey(o.ParentSheetIndex))
                && !IsSerializable(o);
        }

        private static bool IsSerializable(Item o)
        {
            return new[] { typeof(Item), typeof(Object), typeof(ColoredObject) }.Contains(o.GetType());
        }

        private static void ConvertObjectsInInventory(Farmer farmer, Predicate<Object> condition, Func<Object, Object> convert)
        {
            var count = 0;
            for (var i = 0; i < farmer.Items.Count; ++i)
            {
                var chestObject = farmer.Items[i] as Object;
                if (chestObject == null || !condition(chestObject)) continue;
                farmer.Items[i] = convert(chestObject);
                ++count;
            }

            if (count != 0) Log.SyncColour($"Found {count} objects in inventory.", ConsoleColor.DarkGray);
        }

        private static void ConvertObjectsInLocation(GameLocation location, Predicate<Object> condition, Func<Object, Object> convert)
        {
            var locationObjects = location.Objects;
            var wrongObjectInfos = locationObjects.Where(o => condition(o.Value)).ToList();
            if (wrongObjectInfos.Count != 0)
            {
                Log.SyncColour($"Found {wrongObjectInfos.Count} objects in {location.Name}.", ConsoleColor.DarkGray);
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

        private static void ConvertObjectsInChests(GameLocation location, Predicate<Object> condition, Func<Object, Object> convert)
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
                if (count != 0) Log.SyncColour($"Found {count} objects in {chest.Name}.", ConsoleColor.DarkGray);
            }
        }

        private static void ConvertObjectsInMachines(GameLocation location, Predicate<Object> condition, Func<Object, Object> convert)
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
            if (count != 0) Log.SyncColour($"Found {count} objects in machines.", ConsoleColor.DarkGray);
        }

        private static void ConvertObjectsInBuildings(GameLocation location, Predicate<Object> condition, Func<Object, Object> convert)
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
