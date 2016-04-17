using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.NewMachinesMod.Data;
using Microsoft.Xna.Framework.Graphics;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class CustomMachineBase : MachineBase
    {
        protected CustomMachineBase(int id) : base(id) {}

        protected abstract MachineInformation MachineInformation { get; }
        protected override MachineOutputInformation Output => MachineInformation.Output;

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            var spriteIndex = ParentSheetIndex;
            switch (State)
            {
                case MachineState.Empty:
                    spriteIndex += MachineInformation.Draw?.Empty ?? 0;
                    break;
                case MachineState.Processing:
                    spriteIndex += MachineInformation.Draw?.Working ?? 0;
                    break;
                case MachineState.Ready:
                    spriteIndex += MachineInformation.Draw?.Ready ?? 0;
                    break;
            }
            DrawSprite(spriteIndex, spriteBatch, x, y, alpha);
        }
    }
}
