using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Utils;
using Igorious.StardewValley.NewMachinesMod.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class CustomMachineBase : MachineBase
    {
        protected CustomMachineBase(int id) : base(id) { }

        protected abstract MachineInformation MachineInformation { get; }
        protected override MachineOutputInformation Output => MachineInformation.Output;

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            int spriteDelta;
            string color;
            GetSpriteDeltaAndColor(out spriteDelta, out color);

            if (color == null)
            {
                DrawObject(spriteBatch, x, y, alpha, null, spriteDelta);
            }
            else
            {
                DrawObject(spriteBatch, x, y, alpha);
                DrawObject(spriteBatch, x, y, alpha, ConvertColor(color), spriteDelta);
            }
            DrawHeldObject(spriteBatch, x, y);
        }

        private MachineDraw GetDrawInfo()
        {
            if (heldObject == null) return MachineInformation.Draw;

            var itemDraw = MachineInformation.Output.Items.Values
                .FirstOrDefault(i => i?.ID == heldObject.ParentSheetIndex)?.Draw;
            if (itemDraw != null) return itemDraw;

            return MachineInformation.Draw;
        }

        protected void GetSpriteDeltaAndColor(out int spriteDelta, out string color)
        {
            var draw = GetDrawInfo();
            color = null;
            spriteDelta = 0;

            switch (State)
            {
                case MachineState.Empty:
                    spriteDelta = draw?.Empty ?? 0;
                    break;

                case MachineState.Working:
                    spriteDelta = draw?.Working ?? 0;
                    color = draw?.WorkingColor;
                    break;

                case MachineState.Ready:
                    spriteDelta = draw?.Ready ?? 0;
                    color = draw?.ReadyColor;
                    break;
            }
        }

        private Color? ConvertColor(string color)
        {
            if (color == null) return null;
            if (color != "@") return RawColor.FromHex(color).ToXnaColor();
            if (heldObject == null) return null;
            if (heldObject is ColoredObject) return ((ColoredObject)heldObject).color;
            return DominantColorFinder.GetDominantColor(heldObject.ParentSheetIndex, Game1.objectSpriteSheet, 16, 16);
        }
    }
}
