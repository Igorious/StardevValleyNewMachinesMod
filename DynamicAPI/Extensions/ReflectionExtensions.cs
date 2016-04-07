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
    }
}
