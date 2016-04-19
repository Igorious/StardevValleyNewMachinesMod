using Igorious.StardewValley.DynamicAPI.Interfaces;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace Igorious.StardewValley.DynamicAPI.Objects
{
    public class SmartColoredObject : ColoredObject, ISmartObject
    {
        public SmartColoredObject() { }

        public SmartColoredObject(int parentSheetIndex, int stack, Color color) : base(parentSheetIndex, stack, color) { }

        public override Item getOne()
        {
            return new SmartColoredObject(parentSheetIndex, 1, color)
            {
                quality = quality,
                price = price,
                hasBeenInInventory = hasBeenInInventory,
                hasBeenPickedUpByFarmer = hasBeenPickedUpByFarmer,
                specialVariable = specialVariable,
                Name = Name,
            };
        }
    }
}
