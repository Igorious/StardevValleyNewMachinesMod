﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Objects;
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

        private readonly Dictionary<int, Type> _typeMap = new Dictionary<int, Type>();
        private readonly Dictionary<int, DynamicTypeInfo> _dynamicTypeMap = new Dictionary<int, DynamicTypeInfo>();

        #endregion

        #region	Public Properties

        public IReadOnlyDictionary<int, Type> TypeMap => _typeMap;
        public IReadOnlyDictionary<int, DynamicTypeInfo> DynamicTypeMap => _dynamicTypeMap;

        #endregion

        #region	Public Methods

        /// <summary>
        /// Map game ID to specific class.
        /// </summary>
        public void Map<TObject>(int id) where TObject : SmartObjectBase, new()
        {
            _typeMap.Add(id, typeof(TObject));
        }

        /// <summary>
        /// Map game ID to specific class.
        /// </summary>
        public void Map(DynamicTypeInfo dynamicTypeInfo)
        {
            _dynamicTypeMap.Add(dynamicTypeInfo.ClassID, dynamicTypeInfo);
        }

        /// <summary>
        /// Get mapped ID for specific class.
        /// </summary>
        public int GetID<TObject>() where TObject : Object
        {
            return TypeMap.First(kv => kv.Value == typeof(TObject)).Key;
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
            if (TypeMap.TryGetValue(rawObject.ParentSheetIndex, out type))
            {
                smartObject = (Object)Activator.CreateInstance(type);
            }
            else
            {
                var dynamicType = DynamicTypeMap[rawObject.ParentSheetIndex];
                var ctor = dynamicType.BaseType.GetConstructor(new[] { typeof(int) });
                smartObject = (Object)ctor.Invoke(new object[] { dynamicType.ClassID });
            }
            CopyProperties(rawObject, smartObject);
            return smartObject;
        }

        private Object ToRawObject(Object smartObject)
        {
            var rawObject = new Object(smartObject.TileLocation, smartObject.ParentSheetIndex);
            CopyProperties(smartObject, rawObject);
            return rawObject;
        }

        private void CopyProperties(Object from, Object to)
        {
            to.heldObject = from.heldObject;
            to.minutesUntilReady = from.minutesUntilReady;
            to.readyForHarvest = from.readyForHarvest;
            to.TileLocation = from.TileLocation;
        }

        private void CovertToSmartObjects()
        {
            var sw = Stopwatch.StartNew();
            ConvertObjects(Game1.currentLocation, IsRawObject, ToSmartObject);
            Log.SyncColour($"Convertion ('smart') finished: {sw.ElapsedMilliseconds} ms", ConsoleColor.DarkGray);
        }

        private void CovertToRawObjects()
        {
            var sw = Stopwatch.StartNew();
            foreach (var gameLocation in Game1.locations)
            {
                ConvertObjects(gameLocation, IsSmartObject, ToRawObject);
            }
            Log.SyncColour($"Convertion ('raw') finished: {sw.ElapsedMilliseconds} ms", ConsoleColor.DarkGray);
        }

        private bool IsRawObject(Object o)
        {
            return TypeMap.ContainsKey(o.ParentSheetIndex) && (o.GetType() != TypeMap[o.ParentSheetIndex])
                || DynamicTypeMap.ContainsKey(o.ParentSheetIndex) && !(o is IDynamic);
        }

        private bool IsSmartObject(Object o)
        {
            return (TypeMap.ContainsKey(o.ParentSheetIndex) || DynamicTypeMap.ContainsKey(o.ParentSheetIndex))
                && !IsSerializable(o);
        }

        private static bool IsSerializable(Item o)
        {
            return new[] { typeof(Item), typeof(Object), typeof(ColoredObject) }.Contains(o.GetType());
        }

        private void ConvertObjects(GameLocation location, Predicate<Object> condition, Func<Object, Object> convert)
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

            var buildableLocation = location as BuildableGameLocation;
            if (buildableLocation == null) return;

            foreach (var building in buildableLocation.buildings)
            {
                if (building.indoors != null)
                {
                    ConvertObjects(building.indoors, IsSmartObject, ToRawObject);
                }
            }
        }

        #endregion
    }
}
