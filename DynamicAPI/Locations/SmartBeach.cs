using System;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using xTile.Dimensions;
using Rectangle = xTile.Dimensions.Rectangle;

namespace Igorious.StardewValley.DynamicAPI.Locations
{
    public sealed class SmartBeach : Beach, ISmartLocation
    {
        Type ISmartLocation.BaseType { get; } = typeof(Beach);
        public SmartLocationProxy Proxy { get; }

        public SmartBeach() { Proxy = new SmartLocationProxy(this); }

        public override bool performToolAction(Tool t, int tileX, int tileY) => Proxy.PerformToolAction(base.performToolAction, t, tileX, tileY);

        public override bool isActionableTile(int xTile, int yTile, Farmer who) => Proxy.IsActionableTile(base.isActionableTile, xTile, yTile, who);

        public override bool checkAction(Location tileLocation, Rectangle viewport, Farmer who) => Proxy.CheckAction(base.checkAction, tileLocation, viewport, who);

        public override bool isCollidingPosition(Microsoft.Xna.Framework.Rectangle position, Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false)
            => Proxy.IsCollidingPosition(base.isCollidingPosition, position, viewport, isFarmer, damagesFarmer, glider, character, pathfinding, projectile, ignoreCharacterRequirement);

        public override bool isCollidingPosition(Microsoft.Xna.Framework.Rectangle position, Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character)
            => Proxy.IsCollidingPosition(base.isCollidingPosition, position, viewport, isFarmer, damagesFarmer, glider, character);

        public override bool isTileOccupied(Vector2 tileLocation, string characterToIgnore = "") => Proxy.IsTileOccupied(base.isTileOccupied, tileLocation, characterToIgnore);

        public override bool isTileLocationTotallyClearAndPlaceable(Vector2 v) => Proxy.IsTileLocationTotallyClearAndPlaceable(base.isTileLocationTotallyClearAndPlaceable, v);
    }
}