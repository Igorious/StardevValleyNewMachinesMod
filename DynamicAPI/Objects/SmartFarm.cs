using System;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Utils;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using XRectangle = xTile.Dimensions.Rectangle;

namespace Igorious.StardewValley.DynamicAPI.Objects
{
    public sealed class SmartFarm : Farm, ISmartLocation
    {
        Type ISmartLocation.BaseType { get; } = typeof(Farm);

        public override bool isActionableTile(int xTile, int yTile, Farmer who)
        {
            var rectangle = new Rectangle(xTile * Game1.tileSize, yTile * Game1.tileSize, Game1.tileSize, Game1.tileSize);
            foreach (var o in Objects.Values.Where(o => o is ISmartObject))
            {
                if (o.getBoundingBox(o.TileLocation).Intersects(rectangle))
                {
                    return o.isActionable(who);
                }
            }
            return base.isActionableTile(xTile, yTile, who);
        }

        public override bool checkAction(Location tileLocation, XRectangle viewport, Farmer who)
        {
            var rectangle = new Rectangle(tileLocation.X * Game1.tileSize, tileLocation.Y * Game1.tileSize, Game1.tileSize, Game1.tileSize);
            foreach (var o in Objects.Values.Where(o => o is ISmartObject))
            {
                if (o.getBoundingBox(o.TileLocation).Intersects(rectangle))
                {
                    return o.checkForAction(who);
                }
            }
            return base.checkAction(tileLocation, viewport, who);
        }

        private bool IsCollidingWithSmartObject(Rectangle position)
        {
            foreach (var o in Objects.Values.Where(o => o is ISmartObject))
            {
                if (o.getBoundingBox(o.TileLocation).Intersects(position)) return true;
            }
            return false;
        }

        public override bool isCollidingPosition(Rectangle position, XRectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false)
        {
            if (IsCollidingWithSmartObject(position)) return true;
            return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, false, false);
        }

        public override bool isCollidingPosition(Rectangle position, XRectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character)
        {
            if (character == null || character.willDestroyObjectsUnderfoot)
            {
                if (IsCollidingWithSmartObject(position)) return true;
            }
            return base.isCollidingPosition(position, viewport, isFarmer, damagesFarmer, glider, character);
        }

        public override bool isTileOccupied(Vector2 tileLocation, string characterToIgnore = "")
        {
            var rectangle = new Rectangle((int) tileLocation.X * Game1.tileSize + 1, (int) tileLocation.Y * Game1.tileSize + 1, Game1.tileSize - 2, Game1.tileSize - 2);
            if (IsCollidingWithSmartObject(rectangle)) return true;
            return base.isTileOccupied(tileLocation, characterToIgnore);
        }

        public override bool isTileLocationTotallyClearAndPlaceable(Vector2 v)
        {
            var tiledVector = v * Game1.tileSize;
            var point = new Point((int)tiledVector.X + Game1.tileSize / 2, (int)tiledVector.Y + Game1.tileSize / 2);

            foreach (var o in Objects.Values.Where(o => o is ISmartObject))
            {
                if (o.getBoundingBox(o.TileLocation).Contains(point)) return false;
            }
            return base.isTileLocationTotallyClearAndPlaceable(v);
        }
    }
}
