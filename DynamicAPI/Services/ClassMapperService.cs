using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services.Internal;
using Igorious.StardewValley.DynamicAPI.Utils;
using StardewModdingAPI.Events;
using StardewValley;
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
            ActivateMapping();

            TimeEvents.TimeOfDayChanged += (s, e) =>
            {
                if (Game1.timeOfDay != 610) return;
                if (!ActivateMapping()) return;
                Log.ImportantInfo("FORCED ACTIVATION!");
                CovertToSmartObjectsInWorld();
            };

            TimeEvents.OnNewDay += (s, e) =>
            {
                if (e.IsNewDay) return;
                if (!DeactivateMapping()) return;
                CovertToRawObjectsInWorld(); // Allow use native serializer.
            };
        }

        private static ClassMapperService _instance;

        public static ClassMapperService Instance => _instance ?? (_instance = new ClassMapperService());

        #endregion

        #region Private Data

        private bool IsActivated { get; set; }

        private readonly Dictionary<int, Type> _itemTypeMap = new Dictionary<int, Type>();
        private readonly Dictionary<int, Type> _craftableTypeMap = new Dictionary<int, Type>();
        private readonly Dictionary<int, DynamicTypeInfo> _dynamicCraftableTypeMap = new Dictionary<int, DynamicTypeInfo>();

        #endregion

        #region	Properties

        public IReadOnlyDictionary<int, Type> ItemTypeMap => _itemTypeMap;
        public IReadOnlyDictionary<int, Type> CraftableTypeMap => _craftableTypeMap;
        public IReadOnlyDictionary<int, DynamicTypeInfo> DynamicCraftableTypeMap => _dynamicCraftableTypeMap;

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
        
        public Object ToSmartObject(Object rawObject)
        {
            var smartObject = rawObject.bigCraftable
                ? CraftableToSmartObject(rawObject)
                : ItemToSmartObject(rawObject);
            Cloner.Instance.CopyProperties(rawObject, smartObject);
            return smartObject;
        }

        #endregion

        #region	Auxiliary Methods

        #region Handlers

        private void OnMenuClosed(object sender, EventArgsClickableMenuClosed args)
        {
            if (!(args.PriorMenu is SaveGameMenu)) return;
            if (!ActivateMapping()) return;
            CovertToSmartObjectsInWorld();
        }

        private void OnLocationObjectsChanged(object sender, EventArgsLocationObjectsChanged eventArgsLocationObjectsChanged)
        {
            CovertToSmartObjectsInLocation();
        }

        #endregion

        #region Conversion

        private Object CraftableToSmartObject(Object rawObject)
        {
            var craftableID = rawObject.ParentSheetIndex;

            Type type;
            if (_craftableTypeMap.TryGetValue(craftableID, out type))
            {
                var ctor = type.GetConstructor(new Type[] { });
                if (ctor != null) return (Object)ctor.Invoke(new object[] { });
                Log.Error($"Can't find .ctor (static) for CraftableID={craftableID}.");
                return rawObject;
            }

            DynamicTypeInfo dynamicTypeInfo;
            if (_dynamicCraftableTypeMap.TryGetValue(craftableID, out dynamicTypeInfo))
            {
                var ctor = dynamicTypeInfo.BaseType.GetConstructor(new[] { typeof(int) });
                if (ctor != null) return (Object)ctor.Invoke(new object[] { dynamicTypeInfo.ClassID });
                Log.Error($"Can't find .ctor (dynamic) for CraftableID={craftableID}.");
                return rawObject;
            }

            Log.Error($"Can't find mapping for CraftableID={craftableID}.");
            return rawObject;
        }

        private Object ItemToSmartObject(Object rawObject)
        {
            var itemID = rawObject.ParentSheetIndex;
            var type = _itemTypeMap[itemID];
            var ctor = type.GetConstructor(new Type[] { });
            if (ctor != null) return (Object)ctor.Invoke(new object[] { });

            Log.Error($"Can't find .ctor (static) for ItemID={itemID}.");
            return rawObject;
        }

        private void CovertToSmartObjectsInLocation()
        {
            var sw = Stopwatch.StartNew();
            Log.Info($"Activating objects (changes in {Game1.currentLocation?.Name})...");
            Traverser.ConvertObjectsInLocation(Game1.currentLocation, IsRawObject, ToSmartObject);
            Game1.getAllFarmers().ForEach(f => Traverser.ConvertObjectsInInventory(f, IsRawObject, ToSmartObject));
            Log.Info($"Convertion ('smart') finished: {sw.ElapsedMilliseconds} ms");
        }

        private void CovertToRawObjectsInWorld()
        {                           
            var sw = Stopwatch.StartNew();
            Log.Info("Deactivating objects in world...");
            foreach (var gameLocation in Game1.locations)
            {
                Traverser.ConvertObjectsInLocation(gameLocation, IsSmartObject, Cloner.Instance.ToRawObject);
            }
            Game1.getAllFarmers().ForEach(f => Traverser.ConvertObjectsInInventory(f, IsSmartObject, Cloner.Instance.ToRawObject));
            Log.Info($"Convertion ('raw') finished: {sw.ElapsedMilliseconds} ms");
        }

        private void CovertToSmartObjectsInWorld()
        {                           
            var sw = Stopwatch.StartNew();
            Log.Info("Activating objects in world...");
            foreach (var gameLocation in Game1.locations)
            {
                Traverser.ConvertObjectsInLocation(gameLocation, IsRawObject, ToSmartObject);
            }
            Game1.getAllFarmers().ForEach(f => Traverser.ConvertObjectsInInventory(f, IsRawObject, ToSmartObject));
            Log.Info($"Convertion ('smart') finished: {sw.ElapsedMilliseconds} ms");
        }

        #endregion

        #region Checks

        private bool IsRawCraftable(Object o)
        {
            if (!o.bigCraftable) return false;
            var craftableID = o.ParentSheetIndex;

            Type type;
            if (_craftableTypeMap.TryGetValue(craftableID, out type))
            {
                return (o.GetType() != type);
            }

            DynamicTypeInfo dynamicTypeInfo;
            if (_dynamicCraftableTypeMap.TryGetValue(craftableID, out dynamicTypeInfo))
            {
                return (o.GetType() != dynamicTypeInfo.BaseType);
            }

            return false;
        }

        private bool IsRawItem(Object o)
        {
            if (o.bigCraftable) return false;
            var itemID = o.ParentSheetIndex;

            Type type;
            if (_itemTypeMap.TryGetValue(itemID, out type))
            {
                return (o.GetType() != type);
            }

            return false;
        }

        private bool IsRawObject(Object o)
        {
            return IsRawCraftable(o) || IsRawItem(o);
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

        #endregion

        #region Activation

        private bool ActivateMapping()
        {
            if (IsActivated) return false;
            IsActivated = true;
            LocationEvents.LocationObjectsChanged += OnLocationObjectsChanged;
            MenuEvents.MenuClosed -= OnMenuClosed;
            Log.ImportantInfo("Class mapping activated.");
            return true;
        }

        private bool DeactivateMapping()
        {
            if (!IsActivated) return false;
            IsActivated = false;
            LocationEvents.LocationObjectsChanged -= OnLocationObjectsChanged;
            MenuEvents.MenuClosed += OnMenuClosed; // Prevent issue with crash after finishing bundle.
            Log.ImportantInfo("Class mapping deactivated.");
            return true;
        }

        #endregion

        #endregion
    }
}