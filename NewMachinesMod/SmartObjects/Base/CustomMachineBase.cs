using Igorious.StardewValley.DynamicAPI.Interfaces;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class CustomMachineBase : MachineBase
    {
        protected CustomMachineBase(int id) : base(id) {}

        protected abstract IMachine Configuration { get; }
        protected override IMachineOutput MachineOutput => Configuration;
    }
}
