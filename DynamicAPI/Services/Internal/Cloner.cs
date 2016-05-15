using System;
using System.Linq;
using System.Reflection;
using Igorious.StardewValley.DynamicAPI.Extensions;
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

        public void CopyData<T>(T from, T to) => CopyData(from, to, typeof(T));

        public void CopyData(object from, object to, Type type)
        {
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanRead && p.CanWrite);
            foreach (var property in properties)
            {
                property.SetValue(to, property.GetValue(from));
            }

            var fields = type.GetAllFields().Where(f => !f.IsInitOnly && !f.IsLiteral);
            foreach (var field in fields)
            {
                field.SetValue(to, field.GetValue(from));
            }
        }

        public void CopyData(Object from, Object to)
        {
            CopyData<Object>(from, to);
            to.SetColor(from.GetColor());
        }

        #endregion
    }
}