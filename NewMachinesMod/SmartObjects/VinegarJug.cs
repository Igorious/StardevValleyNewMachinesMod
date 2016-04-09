using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class VinegarJug : CustomMachineBase
    {
        private static readonly int ID = ClassMapperService.Instance.GetID<VinegarJug>();

        public VinegarJug() : base(ID) {}

        protected override IMachine Configuration => NewMachinesMod.Config.VinegarJug;

        protected override void PlayDropInSounds()
        {
            PlaySound(Sound.Ship);
            PlaySound(Sound.Bubbles);
        }
    }
}