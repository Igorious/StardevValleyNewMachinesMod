using System;
using Igorious.StardewValley.DynamicAPI.Locations;

namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface ISmartLocation
    {
        Type BaseType { get; }
        SmartLocationProxy Proxy { get; }
    }
}