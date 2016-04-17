using System;
using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Objects
{
    public abstract class SmartObjectBase : Object
    {
        private static readonly Dictionary<Sound, string> SoundNames = new Dictionary<Sound, string>
        {
            {Sound.Bubbles, "bubbles"},
            {Sound.Ship, "Ship"},
        };

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
                return true;
            }
            else
            {
                return PerformDropIn(item, farmer);
            }
        }

        protected void PutItem(int itemID, int count, int itemQuality = 0, string overridedName = null, int? overridedPrice = null, bool isColored = false)
        {
            heldObject = isColored? new ColoredObject(itemID, count, Color.White) : new Object(itemID, count);          
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
    }
}
