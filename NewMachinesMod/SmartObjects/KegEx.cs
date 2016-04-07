using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class KegEx : OverridedMachineBase
    {
        public KegEx() : base(ObjectMapper.GetID<KegEx>()) {}
        protected override IMachineOutput Configuration => NewMachinesMod.Config.KegEx;
    }
}
