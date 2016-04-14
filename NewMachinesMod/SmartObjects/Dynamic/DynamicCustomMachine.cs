using System.Linq;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Dynamic
{
    public sealed class DynamicCustomMachine : CustomMachineBase, IDynamic
    {
        private int ClassID { get; }

        public DynamicCustomMachine(int classID) : base(classID)
        {
            ClassID = classID;
        }

        protected override IMachine Configuration => NewMachinesMod.Config.SimpleMachines.First(m => m.ID == ClassID);
    }
}
