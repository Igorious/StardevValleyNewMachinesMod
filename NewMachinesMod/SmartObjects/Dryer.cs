using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public class Dryer : CustomMachineBase
    {
        public Dryer() : base(ClassMapperService.Instance.GetID<Dryer>()) { }
        protected override IMachine Configuration => NewMachinesMod.Config.Dryer;
    }
}
