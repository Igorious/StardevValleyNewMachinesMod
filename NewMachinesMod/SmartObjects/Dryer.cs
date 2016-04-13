using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public class Dryer : CustomMachineBase
    {
        private static readonly int ID = ClassMapperService.Instance.GetID<Dryer>();

        public Dryer() : base(ID) { }

        protected override IMachine Configuration => NewMachinesMod.Config.Dryer;
    }
}
