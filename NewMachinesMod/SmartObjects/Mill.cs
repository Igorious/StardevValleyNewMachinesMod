using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class Mill : CustomMachineBase
    {
        public Mill() : base(ClassMapperService.Instance.GetID<Mill>()) {}
        protected override IMachine Configuration => NewMachinesMod.Config.Mill;
    }
}