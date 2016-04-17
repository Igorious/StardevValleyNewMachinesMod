using Igorious.StardewValley.NewMachinesMod.Data;
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

        protected virtual bool CanPerformDropInOverrided(Object item, Farmer farmer)
        {
            return base.CanPerformDropIn(item, farmer);
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
