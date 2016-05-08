﻿using Igorious.StardewValley.NewMachinesMod.Data;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class OverridedMachineBase : MachineBase
    {
        protected OverridedMachineBase(int id) : base(id) { }

        private bool? UsedOverride { get; set; }

        protected abstract OverridedMachineInformation MachineInformation { get; }
        protected override MachineOutputInformation Output => MachineInformation.Output;

        protected sealed override bool CanPerformDropIn(Object item, Farmer farmer)
        {
            if (heldObject != null)
            {
                UsedOverride = null;
                return false;
            }

            if (CanPerformDropInOverrided(item, farmer))
            {
                UsedOverride = true;
                return true;
            }

            if (CanPerformDropInRaw(item, farmer))
            {
                UsedOverride = false;
                return true;
            }

            UsedOverride = null;
            return false;
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            var draw = MachineInformation.Draw;
            if (draw != null)
            {
                if (readyForHarvest)
                {
                    showNextIndex = (draw.Ready != 0);
                }
                else if (heldObject != null)
                {
                    showNextIndex = (draw.Working != 0);
                }
                else
                {
                    showNextIndex = (draw.Empty != 0);
                }
            }
            base.draw(spriteBatch, x, y, alpha);         
        }

        protected virtual bool CanPerformDropInOverrided(Object item, Farmer farmer)
        {
            return (MachineInformation.IgnoredItems?.Contains(item.ParentSheetIndex) != true) && base.CanPerformDropIn(item, farmer);
        }

        protected virtual bool PerformDropInOverrided(Object item, Farmer farmer)
        {
            return base.PerformDropIn(item, farmer);
        }

        protected sealed override bool PerformDropIn(Object item, Farmer farmer)
        {
            switch (UsedOverride)
            {
                case true: return PerformDropInOverrided(item, farmer);
                case false: return PerformDropInRaw(item, farmer);
                default: return false;
            }
        }
    }
}
