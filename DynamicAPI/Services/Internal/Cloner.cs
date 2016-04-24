using System.Linq;
using System.Reflection;
using StardewValley;
using StardewValley.Objects;

namespace Igorious.StardewValley.DynamicAPI.Services.Internal
{
    public sealed class Cloner
    {
        #region Singleton Access

        private Cloner() {}

        private static Cloner _instance;

        public static Cloner Instance => _instance ?? (_instance = new Cloner());

        #endregion

        #region	Public Methods

        public void CopyProperties(Object from, Object to)
        {
            var properties = typeof(Object).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite);
            foreach (var property in properties)
            {
                property.SetValue(to, property.GetValue(@from));
            }

            var fields = typeof(Object).GetFields(BindingFlags.Instance | BindingFlags.Public).Where(f => !f.IsInitOnly);
            foreach (var field in fields)
            {
                field.SetValue(to, field.GetValue(@from));
            }

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
            CopyProperties(smartObject, rawObject);
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