﻿using System;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI
{
    public abstract class SmartObjectBase : Object
    {
        protected SmartObjectBase(int id) : base(Vector2.Zero, id) { }

        private static readonly Object ProbeObject = new Object();

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
            }
            else
            {
                PerformDropIn(item, farmer);
            }
            return true;
        }

        protected void PutItem(int itemID, int count, int itemQuality = 0, string overridedName = null, int? overridedPrice = null)
        {
            heldObject = new Object(itemID, count, quality: itemQuality);
            if (overridedName != null) heldObject.Name = overridedName;
            if (overridedPrice != null) heldObject.Price = overridedPrice.Value;
        }

        protected void PlaySound(string sound)
        {
            Game1.playSound(sound);
        }

        public sealed override bool performToolAction(Tool tool)
        {
            if (tool is Pickaxe) return OnPickaxeAction((Pickaxe)tool);
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

        protected virtual void OnWateringCanAction(WateringCan wateringCan)
        {
            base.performToolAction(wateringCan);
        }

        protected virtual bool CanPerformDropIn(Object item, Farmer farmer) => CanPerformDropInRaw(item, farmer);

        protected virtual void PerformDropIn(Object item, Farmer farmer) => PerformDropInRaw(item, farmer);

        protected void PerformDropInRaw(Object item, Farmer farmer)
        {
            base.performObjectDropInAction(item, false, farmer);
        }

        protected bool CanPerformDropInRaw(Object item, Farmer farmer)
        {
            return base.performObjectDropInAction(item, true, farmer);
        }
    }
}