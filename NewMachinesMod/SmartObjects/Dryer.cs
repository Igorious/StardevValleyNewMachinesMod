using System;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects
{
    public class Dryer : CustomMachineBase
    {
        private static readonly int ID = ClassMapperService.Instance.GetID<Dryer>();

        public Dryer() : base(ID) { }

        protected override IMachine Configuration => NewMachinesMod.Config.Dryer;

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            var spriteIndex = ParentSheetIndex;
            if (readyForHarvest)
            {
                spriteIndex += 2;
            }
            else if (heldObject != null)
            {
                spriteIndex += 1;
            }
            DrawSprite(spriteIndex, false, spriteBatch, x, y, alpha);
        }

        private void DrawSprite(int spriteIndex, bool useShake, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            var v1 = getScale() * Game1.pixelZoom;
            var v2 = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize, y * Game1.tileSize - Game1.tileSize));
            var destinationX = v2.X - v1.X / 2 + (useShake && shakeTimer > 0? Game1.random.Next(-1, 2) : 0);
            var destinationY = v2.Y - v1.Y / 2 + (useShake && shakeTimer > 0? Game1.random.Next(-1, 2) : 0);
            var width = Game1.tileSize + v1.X;
            var height = Game1.tileSize * 2 + v1.Y / 2;
            var destinationRectangle = new Rectangle((int)destinationX, (int)destinationY, (int)width, (int)height);
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, getSourceRectForBigCraftable(spriteIndex), Color.White * alpha, 0, Vector2.Zero, SpriteEffects.None, Math.Max(0, ((y + 1) * Game1.tileSize - Game1.pixelZoom * 6) / 10000f));
            if (!readyForHarvest) return;

            var num = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250), 2));
            var depth = (float)((y + 1) * Game1.tileSize / 10000.0 + tileLocation.X / 10000.0);
            spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize - 8, y * Game1.tileSize - Game1.tileSize * 3 / 2 - 16 + num)), new Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0, Vector2.Zero, 4, SpriteEffects.None, depth - 0.001f);
            spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize + Game1.tileSize / 2, y * Game1.tileSize - Game1.tileSize - Game1.tileSize / 8 + num)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, heldObject.parentSheetIndex, 16, 16), Color.White * 0.75f, 0, new Vector2(8, 8), Game1.pixelZoom, SpriteEffects.None, depth);
        }
    }
}
