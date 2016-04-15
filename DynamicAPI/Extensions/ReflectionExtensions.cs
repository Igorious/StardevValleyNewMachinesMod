using System;
using System.Linq;
using System.Reflection;

namespace Igorious.StardewValley.DynamicAPI.Extensions
{
    public static class ReflectionExtensions
    {
        public static T GetField<T>(this object o, string fieldName) where T : class
        {
            var fieldInfo = o.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var value = fieldInfo?.GetValue(o);
            return value as T;
        }

        public static bool IsImplementGenericInterface(this Type t, Type genericInterface)
        {
            return t.GetInterfaces().Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == genericInterface);
        }
    }
}
