using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class KegEx : OverridedMachineBase
    {
        public KegEx() : base(ClassMapperService.Instance.GetID<KegEx>()) {}
        protected override IMachineOutput MachineOutput => NewMachinesMod.Config.KegEx;
    }
}
