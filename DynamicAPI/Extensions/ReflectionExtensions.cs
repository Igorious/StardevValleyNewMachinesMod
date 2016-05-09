using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Igorious.StardewValley.DynamicAPI.Extensions
{
    public static class ReflectionExtensions
    {
        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            if (type == null) return Enumerable.Empty<FieldInfo>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.Static | BindingFlags.Instance |
                                       BindingFlags.DeclaredOnly;
            return type.GetFields(flags).Concat(GetAllFields(type.BaseType));
        }

        public static T GetField<T>(this object o, string fieldName) where T : class
        {
            var fieldInfo = o.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var value = fieldInfo?.GetValue(o);
            return value as T;
        }

        public static void SetField<T>(this object o, string fieldName, T value) where T : class
        {
            var fieldInfo = o.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            fieldInfo?.SetValue(o, value);
        }

        public static bool IsImplementGenericInterface(this Type t, Type genericInterface)
        {
            return t.GetInterfaces().Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == genericInterface);
        }
    }
}
