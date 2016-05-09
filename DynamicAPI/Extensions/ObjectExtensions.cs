using Igorious.StardewValley.DynamicAPI.Objects;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace Igorious.StardewValley.DynamicAPI.Extensions
{
    public static class ObjectExtensions
    {
        public static Color? GetColor(this Object o)
        {
            if (o is ColoredObject) return ((ColoredObject)o).color;
            if (o is SmartObject) return ((SmartObject)o).Color;
            return null;
        }

        public static void SetColor(this Object o, Color? color)
        {
            if (o is ColoredObject && color != null)
            {
                ((ColoredObject)o).color = color.Value;
            }
            else if (o is SmartObject)
            {
                ((SmartObject)o).Color = color;
            }
        }
    }
}