using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI.Events;
using StardewValley;
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

        private static ClassMapperService _instance;

        public static ClassMapperService Instance => _instance ?? (_instance = new ClassMapperService());

        #endregion

        #region Private Data

        private readonly Dictionary<int, Type> _typeMap = new Dictionary<int, Type>();

        #endregion

        #region	Public Properties

        public IReadOnlyDictionary<int, Type> TypeMap => _typeMap;

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
        /// Get mapped ID for specific class.
        /// </summary>
        public int GetID<TObject>() where TObject : Object
        {
            return TypeMap.First(kv => kv.Value == typeof(TObject)).Key;
        }

        #endregion

        #region	Auxiliary Methods

        private void OnLocationObjectsChanged(object sender, EventArgsLocationObjectsChanged eventArgsLocationObjectsChanged)
        {
            CovertToSmartObjects();
        }

        private Object ToSmartObject(Object rawObject)
        {
            var type = TypeMap[rawObject.ParentSheetIndex];
            var smartObject = (Object)Activator.CreateInstance(type);
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
            ConvertObjects(Game1.currentLocation, o => (o.GetType() != TypeMap[o.ParentSheetIndex]), ToSmartObject);
        }

        private void CovertToRawObjects()
        {
            foreach (var gameLocation in Game1.locations)
            {
                ConvertObjects(gameLocation, o => (o.GetType() != typeof(Object)), ToRawObject);
            }
        }

        private void ConvertObjects(GameLocation location, Predicate<Object> condition, Func<Object, Object> convert)
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
