using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class VinegarJug : CustomMachineBase
    {
        public VinegarJug() : base(ClassMapperService.Instance.GetID<VinegarJug>()) {}
        protected override IMachine Configuration => NewMachinesMod.Config.VinegarJug;
    }
}