using Igorious.StardewValley.DynamicAPI.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;

namespace Igorious.StardewValley.DynamicAPI.Objects
{
    public class SmartColoredObject : ColoredObject, ISmartObject
    {
        public SmartColoredObject() { }

        public SmartColoredObject(int parentSheetIndex, int stack, Color color) : base(parentSheetIndex, stack, color) { }

        public override Item getOne()
        {
            return new SmartColoredObject(parentSheetIndex, 1, color)
            {
                quality = quality,
                price = price,
                hasBeenInInventory = hasBeenInInventory,
                hasBeenPickedUpByFarmer = hasBeenPickedUpByFarmer,
                specialVariable = specialVariable,
                Name = Name,
            };
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber)
        {
            DrawShadow(spriteBatch, location, scaleSize, layerDepth);
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber);
        }

        private void DrawShadow(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float layerDepth)
        {
            spriteBatch.Draw(
                Game1.shadowTexture, 
                location + new Vector2(Game1.tileSize / 2f, Game1.tileSize * 3 / 4f), 
                Game1.shadowTexture.Bounds, 
                Color.White * 0.5f, 
                0, 
                new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 
                3, 
                SpriteEffects.None, 
                layerDepth - 0.001f);
        }
    }
}
