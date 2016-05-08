using System;
using System.Linq;
using System.Reflection;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Services.Internal
{
    public sealed class Cloner
    {
        #region Singleton Access

        private Cloner() { }

        private static Cloner _instance;

        public static Cloner Instance => _instance ?? (_instance = new Cloner());

        #endregion

        #region	Public Methods

        public void CopyData<T>(T from, T to) => CopyData(@from, to, typeof(T));

        public void CopyData(object from, object to, Type type)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite);
            foreach (var property in properties)
            {
                property.SetValue(to, property.GetValue(from));
            }

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public).Where(f => !f.IsInitOnly);
            foreach (var field in fields)
            {
                field.SetValue(to, field.GetValue(from));
            }
        }

        public void CopyData(Object from, Object to)
        {
            CopyData<Object>(from, to);
            if (from is ColoredObject && to is ColoredObject)
            {
                ((ColoredObject)to).color = ((ColoredObject)@from).color;
            }
        }

        public Object ToRawObject(Object smartObject)
        {
            var rawObject = (smartObject is ColoredObject)
                ? ColoredToRawObject((ColoredObject)smartObject)
                : (smartObject.bigCraftable
                    ? CraftableToRawObject(smartObject)
                    : ItemToRawObject(smartObject));
            CopyData(smartObject, rawObject);
            return rawObject;
        }

        #endregion

        #region	Auxiliary Methods

        private static Object ColoredToRawObject(ColoredObject smartObject)
        {
            return new ColoredObject(smartObject.ParentSheetIndex, smartObject.stack, smartObject.color);
        }

        private static Object CraftableToRawObject(Object smartObject)
        {
            return new Object(smartObject.TileLocation, smartObject.ParentSheetIndex);
        }

        private static Object ItemToRawObject(Object smartObject)
        {
            return new Object(smartObject.ParentSheetIndex, smartObject.stack);
        }

        #endregion
    }
}