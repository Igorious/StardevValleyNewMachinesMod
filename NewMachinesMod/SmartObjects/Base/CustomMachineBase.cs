using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Microsoft.Xna.Framework.Graphics;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class CustomMachineBase : MachineBase
    {
        protected CustomMachineBase(int id) : base(id) {}

        protected abstract IMachine Configuration { get; }
        protected override IMachineOutput MachineOutput => Configuration;

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            var spriteIndex = ParentSheetIndex;
            switch (State)
            {
                case MachineState.Empty:
                    spriteIndex += Configuration.Draw?.Empty ?? 0;
                    break;
                case MachineState.Processing:
                    spriteIndex += Configuration.Draw?.Processing ?? 0;
                    break;
                case MachineState.Ready:
                    spriteIndex += Configuration.Draw?.Ready ?? 0;
                    break;
            }
            DrawSprite(spriteIndex, spriteBatch, x, y, alpha);
        }
    }
}
