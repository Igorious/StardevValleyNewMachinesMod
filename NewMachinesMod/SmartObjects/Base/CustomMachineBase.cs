using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Interfaces;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class CustomMachineBase : MachineBase
    {
        protected CustomMachineBase(int id) : base(id) {}

        protected abstract IMachine Configuration { get; }
        protected override OutputInfo Output => Configuration.Output;
        protected override int? MinutesUntilReady => Configuration.MinutesUntilReady;
    }
}
