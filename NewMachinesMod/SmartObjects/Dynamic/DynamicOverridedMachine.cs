using System.Linq;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Dynamic
{
    public sealed class DynamicOverridedMachine : OverridedMachineBase, IDynamic
    {
        private int ClassID { get; }

        public DynamicOverridedMachine(int classID) : base(classID)
        {
            ClassID = classID;
        }

        protected override IMachineOutput MachineOutput => NewMachinesMod.Config.MachineOverrides.First(m => m.ID == ClassID);
    }
}
