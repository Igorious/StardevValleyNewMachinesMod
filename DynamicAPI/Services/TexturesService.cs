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
using IDrawable = Igorious.StardewValley.DynamicAPI.Interfaces.IDrawable;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public enum Texture
    {
        Craftables = 1,
        Items = 2,
        Crops = 3,
        Trees = 4,
    }

    public sealed class TexturesService
    {
        #region Private Data

        private string RootPath { get; }

        private readonly Dictionary<int, int> _craftableSpriteOverrides = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _itemSpriteOverrides = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _cropSpriteOverrides = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _treeSpriteOverrides = new Dictionary<int, int>();
        private bool NeedOverrideIridiumQualityStar { get; set; }

        #endregion

        #region	Constructor

        public TexturesService(string rootPath)
        {
            RootPath = rootPath;
            GameEvents.LoadContent += OnLoadContent;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Override sprites in specific texture.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="drawable"></param>
        public void Override(Texture texture, IDrawable drawable)
        {
            if (drawable.ResourceIndex == null) return;

            var allOverrides = new[] { null, _craftableSpriteOverrides, _itemSpriteOverrides, _cropSpriteOverrides, _treeSpriteOverrides };
            var overrides = allOverrides[(int)texture];
            if (overrides == null) return;

            for (var i = 0; i < drawable.ResourceLength; ++i)
            {
                if (drawable.ResourceIndex == null) continue;

                var key = drawable.TextureIndex + i;
                var newValue = drawable.ResourceIndex.Value + i;
                int oldValue;
                if (!overrides.TryGetValue(key, out oldValue))
                {
                    overrides.Add(key, newValue);
                }
                else if (newValue != oldValue)
                {
                    Log.SyncColour($"Texture for ${drawable.GetType().Name} already has another mapping {key}->{oldValue} (current: {newValue})", ConsoleColor.DarkRed);
                }
            }
        }

        public void OverrideIridiumQualityStar()
        {
            NeedOverrideIridiumQualityStar = true;
        }

        #endregion

        #region	Auxiliary Methods

        private void OnLoadContent(object sender, EventArgs eventArgs)
        {
            OverrideSprites();
            GameEvents.LoadContent -= OnLoadContent;
        }

        private void OverrideSprites()
        {
            OverrideTexture(ref Game1.bigCraftableSpriteSheet, @"Resources\Craftable.png", _craftableSpriteOverrides, 16, 32);
            OverrideTexture(ref Game1.objectSpriteSheet, @"Resources\Items.png", _itemSpriteOverrides, 16, 16);
            OverrideTexture(ref Game1.cropSpriteSheet, @"Resources\Crops.png", _cropSpriteOverrides, 128, 32);

            new FruitTree().loadSprite();
            OverrideTexture(ref FruitTree.texture, @"Resources\Trees.png", _treeSpriteOverrides, 432, 80);

            if (!NeedOverrideIridiumQualityStar) return;
            Log.SyncColour($"[NMM]: Using overrides from \"Resources\\Other.png\"...", ConsoleColor.DarkGray);
            using (var imageStream = new FileStream(Path.Combine(RootPath, @"Resources\Other.png"), FileMode.Open))
            {
                var overrides = Texture2D.FromStream(Game1.graphics.GraphicsDevice, imageStream);
                var data = new Color[8 * 8];
                overrides.GetData(0, new Rectangle(0, 0, 8, 8), data, 0, data.Length);
                Game1.mouseCursors.SetData(0, new Rectangle(338 + 2 * 8, 400, 8, 8), data, 0, data.Length);
            }
        }

        private void OverrideTexture(ref Texture2D originalTexture, string overridingTexturePath, Dictionary<int, int> spriteOverrides, int gridWidth, int gridHeight)
        {
            if (spriteOverrides.Count == 0) return;

            Log.SyncColour($"[NMM]: Using overrides from \"{overridingTexturePath}\"...", ConsoleColor.DarkGray);
            var maxHeight = GetSourceRectForObject(spriteOverrides.Keys.Max(), originalTexture, gridWidth, gridHeight).Bottom;
            if (maxHeight > originalTexture.Height)
            {
                var allData = new Color[originalTexture.Width * originalTexture.Height];
                originalTexture.GetData(allData);

                var newData = new Color[originalTexture.Width * maxHeight];
                Array.Copy(allData, newData, allData.Length);

                originalTexture = new Texture2D(Game1.graphics.GraphicsDevice, originalTexture.Width, maxHeight);
                originalTexture.SetData(newData);
            }

            using (var imageStream = new FileStream(Path.Combine(RootPath, overridingTexturePath), FileMode.Open))
            {
                var overrideTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, imageStream);
                foreach (var spriteOverride in spriteOverrides)
                {
                    var data = new Color[gridWidth * gridHeight];
                    overrideTexture.GetData(0, GetSourceRectForObject(spriteOverride.Value, overrideTexture, gridWidth, gridHeight), data, 0, data.Length);
                    originalTexture.SetData(0, GetSourceRectForObject(spriteOverride.Key, originalTexture, gridWidth, gridHeight), data, 0, data.Length);
                }
            }
        }

        internal static Rectangle GetSourceRectForObject(int index, Texture2D texture, int gridWidth, int gridHeight)
        {
            return new Rectangle(index % (texture.Width / gridWidth) * gridWidth, index / (texture.Width / gridWidth) * gridHeight, gridWidth, gridHeight);
        }

        #endregion
    }
}