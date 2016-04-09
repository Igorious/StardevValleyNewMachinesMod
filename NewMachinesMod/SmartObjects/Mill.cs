using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;
using Microsoft.Xna.Framework.Graphics;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public sealed class Mill : CustomMachineBase
    {
        private static readonly int ID = ClassMapperService.Instance.GetID<Mill>();

        public Mill() : base(ID) {}

        protected override IMachine Configuration => NewMachinesMod.Config.Mill;

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            showNextIndex = readyForHarvest;
            base.draw(spriteBatch, x, y, alpha);
        }
    }
}