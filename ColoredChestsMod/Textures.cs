using System.IO;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace Igorious.StardewValley.ColoredChestsMod
{
    public static class Textures
    {
        private static Texture2D _chestTint;
        public static Texture2D ChestTint => _chestTint ?? (_chestTint = LoadChestTintTexture());

        private static Texture2D LoadChestTintTexture()
        {
            using (var imageStream = new FileStream(Path.Combine(ColoredChestsMod.RootPath, @"Resources\Chest_Tint.png"), FileMode.Open))
            {
                return Texture2D.FromStream(Game1.graphics.GraphicsDevice, imageStream);
            }
        }
    }
}