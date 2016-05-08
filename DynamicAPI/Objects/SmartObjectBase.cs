using System;
using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Objects
{
    public abstract class SmartObjectBase : Object, ISmartObject
    {
        Type ISmartObject.BaseType { get; } = typeof(Object);

        private static readonly Dictionary<Sound, string> SoundNames = new Dictionary<Sound, string>
        {
            {Sound.Bubbles, "bubbles"},
            {Sound.Ship, "Ship"},
            {Sound.Wand, "wand"},
        };

        protected SmartObjectBase(int id) : base(Vector2.Zero, id) { }

        private static readonly Object ProbeObject = new Object();

        protected virtual int TileWidth { get; } = 1;
        protected virtual int TileHeight { get; } = 1;

        public override Rectangle getBoundingBox(Vector2 tile)
        {
            boundingBox.X = (int)tile.X * Game1.tileSize;
            boundingBox.Y = (int)tile.Y * Game1.tileSize;
            boundingBox.Height = TileHeight * Game1.tileSize;
            boundingBox.Width = TileWidth * Game1.tileSize;
            return boundingBox;
        }

        public override bool canBePlacedHere(GameLocation l, Vector2 tile)
        {
            for (var w = 0; w < TileWidth; ++w)
            for (var h = 0; h < TileHeight; ++h)
            {
                if (!base.canBePlacedHere(l, new Vector2(tile.X + w, tile.Y + h))) return false;
            }
            return true;
        }

        public sealed override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            return justCheckingForActivity? CanDoAction(who) : DoAction(who);
        }

        protected virtual bool CanDoAction(Farmer farmer)
        {
            return base.checkForAction(farmer, true);
        }

        protected virtual bool DoAction(Farmer farmer)
        {
            return base.checkForAction(farmer);
        }

        protected Random GetRandom()
        {
            return new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + Game1.timeOfDay + (int)tileLocation.X * 200 + (int)tileLocation.Y);
        }

        public sealed override bool performObjectDropInAction(Object item, bool isProbe, Farmer farmer)
        {
            var canPerform = CanPerformDropIn(item, farmer);
            if (!canPerform) return false;

            if (isProbe)
            {
                heldObject = ProbeObject;
                return true;
            }
            else
            {
                return PerformDropIn(item, farmer);
            }
        }

        protected void PutItem(int itemID, int count, int itemQuality = 0, string overridedName = null, int? overridedPrice = null, Color? color = null)
        {
            heldObject = color.HasValue ? new SmartColoredObject(itemID, count, color.Value) : new Object(itemID, count);
            heldObject.quality = itemQuality;
            if (overridedName != null) heldObject.Name = string.Format(overridedName, heldObject.Name);
            if (overridedPrice != null) heldObject.Price = overridedPrice.Value;
        }

        protected void PlaySound(Sound sound)
        {
            Game1.playSound(SoundNames[sound]);
        }

        public sealed override bool performToolAction(Tool tool)
        {
            if (tool is Pickaxe) return OnPickaxeAction((Pickaxe)tool);
            if (tool is Axe) return OnAxeAction((Axe)tool);
            if (tool is WateringCan)
            {
                OnWateringCanAction((WateringCan)tool);
                return true;
            }
            return base.performToolAction(tool);
        }

        protected virtual bool OnPickaxeAction(Pickaxe pickaxe)
        {
            return base.performToolAction(pickaxe);
        }

        protected virtual bool OnAxeAction(Axe axe)
        {
            return base.performToolAction(axe);
        }

        protected virtual void OnWateringCanAction(WateringCan wateringCan)
        {
            base.performToolAction(wateringCan);
        }

        protected virtual bool CanPerformDropIn(Object item, Farmer farmer) => CanPerformDropInRaw(item, farmer);

        protected virtual bool PerformDropIn(Object item, Farmer farmer) => PerformDropInRaw(item, farmer);

        protected bool PerformDropInRaw(Object item, Farmer farmer)
        {
            return base.performObjectDropInAction(item, false, farmer);
        }

        protected bool CanPerformDropInRaw(Object item, Farmer farmer)
        {
            base.performObjectDropInAction(item, true, farmer);
            var result = (heldObject != null);
            heldObject = null;
            return result;
        }

        protected void ShowRedMessage(Farmer farmer, string message)
        {
            if (!farmer.IsMainPlayer) return;
            Game1.showRedMessage(message);
        }
    }
}
