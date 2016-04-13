using System;
using Igorious.StardewValley.DynamicAPI.Interfaces;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class DynamicClassInfo
    {
        public string Name { get; set; }
        public Type BaseType { get; set; }

        public static DynamicClassInfo Create<TBaseClass>(string name) where TBaseClass : IDynamic
        {
            return new DynamicClassInfo
            {
                Name = name,
                BaseType = typeof(TBaseClass),
            };
        }
    }
}