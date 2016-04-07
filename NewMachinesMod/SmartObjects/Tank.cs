using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
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
        public FullTank() : base(ObjectMapper.GetID<FullTank>()) { }      
    }

    public class Tank : CustomMachineBase
    {
        public Tank() : this(ObjectMapper.GetID<Tank>()) { }
        protected Tank(int id) : base(id) { }

        protected bool IsEmpty
        {
            get { return (ParentSheetIndex == ObjectMapper.GetID<Tank>()); }
            set { ParentSheetIndex = ObjectMapper.GetID<Tank>() + (value? 0 : 1); }
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

        protected override IMachine Configuration => NewMachinesMod.Config.Tank;

        protected override bool CanPerformDropIn(Object item, Farmer farmer)
        {
            return !IsEmpty && base.CanPerformDropIn(item, farmer);
        }

        protected override void PlayDropInSounds()
        {
            PlaySound(Sound.Ship);
            PlaySound(Sound.Bubbles);
        }
    }
}