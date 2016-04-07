using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using StardewValley;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class OverridedMachineBase : MachineBase
    {
        protected OverridedMachineBase(int id) : base(id) { }

        protected abstract IMachineOutput Configuration { get; }
        protected override OutputInfo Output => Configuration.Output;
        protected override int? MinutesUntilReady => Configuration.MinutesUntilReady;
        private bool? UsedOverride { get; set; }

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

        protected virtual bool CanPerformDropInOverrided(Object item, Farmer farmer)
        {
            return base.CanPerformDropIn(item, farmer);
        }

        protected virtual void PerformDropInOverrided(Object item, Farmer farmer)
        {
            base.PerformDropIn(item, farmer);
        }

        protected sealed override void PerformDropIn(Object item, Farmer farmer)
        {
            if (UsedOverride == true)
            {
                PerformDropInOverrided(item, farmer);
            }
            else if (UsedOverride == false)
            {
                PerformDropInRaw(item, farmer);
            }
        }
    }
}
