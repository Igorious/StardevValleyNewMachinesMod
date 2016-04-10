using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class FullTank : Tank
    {
        private static readonly int ID = ClassMapperService.Instance.GetID<FullTank>();

        public FullTank() : base(ID) { }
    }

    public class Tank : CustomMachineBase
    {
        private static readonly int ID = ClassMapperService.Instance.GetID<Tank>();

        public Tank() : this(ID) { }
        protected Tank(int id) : base(id) { }

        protected bool IsEmpty
        {
            get { return (ParentSheetIndex == ID); }
            set { ParentSheetIndex = ID + (value ? 0 : 1); }
        }

        public override bool minutesElapsed(int minutes, GameLocation environment)
        {
            var result = base.minutesElapsed(minutes, environment);
            if (readyForHarvest) IsEmpty = true;
            return result;
        }

        protected override void OnWateringCanAction(WateringCan wateringCan)
        {
            if (!IsEmpty) return;
            PlaySound(Sound.Bubbles);
            IsEmpty = false;
        }

        protected override bool OnPickaxeAction(Pickaxe pickaxe)
        {
            if (!IsEmpty) PlaySound(Sound.Bubbles);
            IsEmpty = true;
            return base.OnPickaxeAction(pickaxe);
        }

        protected override bool OnAxeAction(Axe axe)
        {
            if (!IsEmpty) PlaySound(Sound.Bubbles);
            IsEmpty = true;
            return base.OnAxeAction(axe);
        }

        protected override IMachine Configuration => NewMachinesMod.Config.Tank;

        protected override bool PerformDropIn(Object item, Farmer farmer)
        {
            if (!IsEmpty) return base.PerformDropIn(item, farmer);
            Game1.showRedMessage(NewMachinesMod.Config.LocalizationStrings[NewMachinesModConfig.LocalizationString.TankRequiresWater]);
            return false;
        }

        protected override void PlayDropInSounds()
        {
            PlaySound(Sound.Ship);
            PlaySound(Sound.Bubbles);
        }
    }
}