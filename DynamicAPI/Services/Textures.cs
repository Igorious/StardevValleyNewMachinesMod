using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class Textures
    {
        #region Private Data

        private static readonly Dictionary<int, int> CraftableSpriteOverrides = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> ItemSpriteOverrides = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> CropSpriteOverrides = new Dictionary<int, int>();
        private static readonly Dictionary<int, int> TreeSpriteOverrides = new Dictionary<int, int>();
        private static bool IsInitialized { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Override craftable sprite with specific ID.
        /// </summary>
        public static void AddCraftableOverride(int xnbIndex, int overrideIndex)
        {
            Initialize();
            CraftableSpriteOverrides.Add(xnbIndex, overrideIndex);
        }

        /// <summary>
        /// Override sprite with specific ID.
        /// </summary>
        public static void AddItemOverride(int xnbIndex, int overrideIndex)
        {
            Initialize();
            ItemSpriteOverrides.Add(xnbIndex, overrideIndex);
        }

        /// <summary>
        /// Override fruit tree sprite with specific ID.
        /// </summary>
        public static void AddTreeOverride(int xnbIndex, int overrideIndex)
        {
            Initialize();
            TreeSpriteOverrides.Add(xnbIndex, overrideIndex);
        }

        /// <summary>
        /// Override fruit tree sprite with specific ID.
        /// </summary>
        public static void AddCropOverride(int xnbIndex, int overrideIndex)
        {
            Initialize();
            CropSpriteOverrides.Add(xnbIndex, overrideIndex);
        }

        #endregion

        #region	Auxiliary Methods

        private static void Initialize()
        {
            if (IsInitialized) return;
            GameEvents.GameLoaded += (s, e) => OverrideSprites();
            IsInitialized = true;
        }

        private static void OverrideSprites()
        {
            OverrideTexture(ref Game1.bigCraftableSpriteSheet, @"Resources\Craftable.png", CraftableSpriteOverrides, 16, 32);
            OverrideTexture(ref Game1.objectSpriteSheet, @"Resources\Items.png", ItemSpriteOverrides, 16, 16);
            OverrideTexture(ref Game1.cropSpriteSheet, @"Resources\Crops.png", CropSpriteOverrides, 128, 32);

            new FruitTree().loadSprite();
            OverrideTexture(ref FruitTree.texture, @"Resources\Trees.png", TreeSpriteOverrides, 432, 80);
        }

        private static void OverrideTexture(ref Texture2D originalTexture, string overridingTexturePath, Dictionary<int, int> spriteOverrides, int gridWidth, int gridHeight)
        {
            if (spriteOverrides.Count == 0) return;

            Log.Debug($"Loading {overridingTexturePath} textures");
            var maxHeight = GetSourceRectForObject(spriteOverrides.Keys.Max(), originalTexture, gridWidth, gridHeight).Bottom;
            if (maxHeight > originalTexture.Height)
            {
                var allData = new Color[originalTexture.Width * originalTexture.Height];
                originalTexture.GetData(allData);

                Log.Debug($"Expected new data: {originalTexture.Width}x{maxHeight}");
                var newData = new Color[originalTexture.Width * maxHeight];
                Array.Copy(allData, newData, allData.Length);

                originalTexture = new Texture2D(Game1.graphics.GraphicsDevice, originalTexture.Width, maxHeight);
                originalTexture.SetData(newData);  
            }

            using (var imageStream = new FileStream(Path.Combine(Configurator.PathOnDisk, overridingTexturePath), FileMode.Open))
            {
                var overrides = Texture2D.FromStream(Game1.graphics.GraphicsDevice, imageStream);
                foreach (var spriteOverride in spriteOverrides)
                {
                    var data = new Color[gridWidth * gridHeight];
                    overrides.GetData(0, GetSourceRectForObject(spriteOverride.Value, originalTexture, gridWidth, gridHeight), data, 0, data.Length);
                    originalTexture.SetData(0, GetSourceRectForObject(spriteOverride.Key, originalTexture, gridWidth, gridHeight), data, 0, data.Length);
                }
            }
            Log.Debug($"Loaded {overridingTexturePath} textures");
        }

        private static Rectangle GetSourceRectForObject(int index, Texture2D texture, int gridWidth, int gridHeight)
        {
            return new Rectangle(index % (texture.Width / gridWidth) * gridWidth, index / (texture.Width / gridWidth) * gridHeight, gridWidth, gridHeight);
        }

        #endregion
    }
}