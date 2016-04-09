using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class KegEx : OverridedMachineBase
    {
        private static readonly int ID = ClassMapperService.Instance.GetID<KegEx>();

        public KegEx() : base(ID) {}

        protected override IMachineOutput Configuration => NewMachinesMod.Config.KegEx;
    }
}
