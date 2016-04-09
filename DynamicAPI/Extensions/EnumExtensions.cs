using System;

namespace Igorious.StardewValley.DynamicAPI.Extensions
{
    public static class EnumExtensions
    {
        public static TEnum ToEnum<TEnum>(this string s)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), s, true);
        }
    }
}
