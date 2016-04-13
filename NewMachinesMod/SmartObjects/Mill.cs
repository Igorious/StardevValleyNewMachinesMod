using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class Mill : CustomMachineBase
    {
        private static readonly int ID = ClassMapperService.Instance.GetID<Mill>();

        public Mill() : base(ID) {}

        protected override IMachine Configuration => NewMachinesMod.Config.Mill;
    }
}