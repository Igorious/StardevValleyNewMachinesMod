using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using StardewModdingAPI.Events;
using StardewValley;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public static class ObjectMapper
    {
        #region Private Data

        private static readonly Dictionary<int, Type> _typeMap = new Dictionary<int, Type>();

        #endregion

        #region	Public Properties

        public static IReadOnlyDictionary<int, Type> TypeMap => _typeMap;

        #endregion

        #region	Public Methods

        /// <summary>
        /// Map game ID to specific class.
        /// </summary>
        public static void AddMapping<TObject>(int id) where TObject : SmartObjectBase, new()
        {
            _typeMap.Add(id, typeof(TObject));
        }

        /// <summary>
        /// Map configuration to specific class.
        /// </summary>
        public static void AddMapping<TObject>(IItem item) where TObject : SmartObjectBase, new()
        {
            AddMapping<TObject>(item.ID);
        }

        /// <summary>
        /// Get mapped ID for specific class.
        /// </summary>
        public static int GetID<TObject>() where TObject : Object
        {
            return TypeMap.First(kv => kv.Value == typeof(TObject)).Key;
        }

        /// <summary>
        /// Allow auto-conversion between classes.
        /// </summary>
        public static void TrackChanges()
        {
            LocationEvents.LocationObjectsChanged += OnLocationObjectsChanged;
            TimeEvents.OnNewDay += (s, e) =>
            {
                if (!e.IsNewDay)
                {
                    LocationEvents.LocationObjectsChanged -= OnLocationObjectsChanged;
                    CovertToRawObjects(); // Allow use native serializer.
                }
                else
                {
                    CovertToSmartObjects(); // Activate objects in farmer house.
                    LocationEvents.LocationObjectsChanged += OnLocationObjectsChanged;
                }
            };
        }

        #endregion

        #region	Auxiliary Methods

        private static void OnLocationObjectsChanged(object sender, EventArgsLocationObjectsChanged eventArgsLocationObjectsChanged)
        {
            CovertToSmartObjects();
        }

        private static Object ToSmartObject(Object rawObject)
        {
            var type = TypeMap[rawObject.ParentSheetIndex];
            var smartObject = (Object)Activator.CreateInstance(type);
            CopyProperties(rawObject, smartObject);
            return smartObject;
        }

        private static Object ToRawObject(Object smartObject)
        {
            var rawObject = new Object(smartObject.TileLocation, smartObject.ParentSheetIndex);
            CopyProperties(smartObject, rawObject);
            return rawObject;
        }

        private static void CopyProperties(Object from, Object to)
        {
            to.heldObject = from.heldObject;
            to.minutesUntilReady = from.minutesUntilReady;
            to.readyForHarvest = from.readyForHarvest;
            to.TileLocation = from.TileLocation;
        }

        private static void CovertToSmartObjects()
        {
            ConvertObjects(Game1.currentLocation, o => (o.GetType() != TypeMap[o.ParentSheetIndex]), ToSmartObject);
        }

        private static void CovertToRawObjects()
        {
            foreach (var gameLocation in Game1.locations)
            {
                ConvertObjects(gameLocation, o => (o.GetType() != typeof(Object)), ToRawObject);
            }
        }

        private static void ConvertObjects(GameLocation location, Predicate<Object> condition, Func<Object, Object> convert)
        {
            var objectInfos = location.Objects;
            var wrongObjectInfos = objectInfos.Where(o => TypeMap.Keys.Contains(o.Value.ParentSheetIndex) && condition(o.Value)).ToList();
            if (wrongObjectInfos.Count == 0) return;

            foreach (var wrongObjectInfo in wrongObjectInfos)
            {
                objectInfos.Remove(wrongObjectInfo.Key);
                var newObject = convert(wrongObjectInfo.Value);
                objectInfos.Add(wrongObjectInfo.Key, newObject);
                newObject.minutesElapsed(0, location);
            }
        }

        #endregion
    }
}
