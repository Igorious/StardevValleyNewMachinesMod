using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using StardewValley;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class OverridedMachineBase : MachineBase
    {
        protected OverridedMachineBase(int id) : base(id) {}

        protected abstract IMachineOutput Configuration { get; }
        protected override OutputInfo Output => Configuration.Output;
        protected override int? MinutesUntilReady => Configuration.MinutesUntilReady;

        protected override bool CanPerformDropIn(Object item, Farmer farmer)
        {
            return base.CanPerformDropIn(item, farmer) || CanPerformDropInRaw(item, farmer);
        }

        protected override void PerformDropIn(Object item, Farmer farmer)
        {
            if (!base.CanPerformDropIn(item, farmer)) PerformDropInRaw(item, farmer);
            base.PerformDropIn(item, farmer);
        }
    }
}
