using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using IDrawable = Igorious.StardewValley.DynamicAPI.Interfaces.IDrawable;
using Texture = Igorious.StardewValley.DynamicAPI.Constants.Texture;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class TexturesService
    {
        #region Private Data

        private string RootPath { get; }

        private readonly Dictionary<int, TextureRect> _craftableSpriteOverrides = new Dictionary<int, TextureRect>();
        private readonly Dictionary<int, TextureRect> _itemSpriteOverrides = new Dictionary<int, TextureRect>();
        private readonly Dictionary<int, TextureRect> _cropSpriteOverrides = new Dictionary<int, TextureRect>();
        private readonly Dictionary<int, TextureRect> _treeSpriteOverrides = new Dictionary<int, TextureRect>();
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

            var key = drawable.TextureIndex;
            var newValue = new TextureRect(drawable.ResourceIndex.Value, drawable.ResourceLength, drawable.ResourceHeight);
            TextureRect oldValue;
            if (!overrides.TryGetValue(key, out oldValue))
            {
                overrides.Add(key, newValue);
            }
            else if (newValue != oldValue)
            {
                Log.Fail($"Texture for ${drawable.GetType().Name} already has another mapping {key}->{oldValue} (current: {newValue})");
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

            using (var file = File.Create(@"D:\temp2.png"))
            {
                Game1.bigCraftableSpriteSheet.SaveAsPng(file, Game1.bigCraftableSpriteSheet.Width, Game1.bigCraftableSpriteSheet.Height);
            }

            new FruitTree().loadSprite();
            OverrideTexture(ref FruitTree.texture, @"Resources\Trees.png", _treeSpriteOverrides, 432, 80);

            if (!NeedOverrideIridiumQualityStar) return;
            Log.Info("Using overrides from \"Resources\\Other.png\"...");
            using (var imageStream = new FileStream(Path.Combine(RootPath, @"Resources\Other.png"), FileMode.Open))
            {
                var overrides = Texture2D.FromStream(Game1.graphics.GraphicsDevice, imageStream);
                var data = new Color[8 * 8];
                overrides.GetData(0, new Rectangle(0, 0, 8, 8), data, 0, data.Length);
                Game1.mouseCursors.SetData(0, new Rectangle(338 + 2 * 8, 400, 8, 8), data, 0, data.Length);
            }
        }

        private void OverrideTexture(ref Texture2D originalTexture, string overridingTexturePath, Dictionary<int, TextureRect> spriteOverrides, int tileWidth, int tileHeight)
        {
            if (spriteOverrides.Count == 0) return;

            Log.Info($"Using overrides from \"{overridingTexturePath}\"...");
            ExtendTexture(ref originalTexture, spriteOverrides, tileWidth, tileHeight);

            using (var imageStream = new FileStream(Path.Combine(RootPath, overridingTexturePath), FileMode.Open))
            {
                var overrideTexture = Texture2D.FromStream(Game1.graphics.GraphicsDevice, imageStream);
                foreach (var spriteOverride in spriteOverrides)
                {
                    var textureRect = spriteOverride.Value;
                    if (textureRect.Height > 1)
                    {
                        var data = new Color[tileWidth * textureRect.Length * tileHeight * textureRect.Height];
                        overrideTexture.GetData(0, GetSourceRectForObject(textureRect.Index, textureRect.Length, textureRect.Height, overrideTexture, tileWidth, tileHeight), data, 0, data.Length);
                        originalTexture.SetData(0, GetSourceRectForObject(spriteOverride.Key, textureRect.Length, textureRect.Height, originalTexture, tileWidth, tileHeight), data, 0, data.Length);
                    }
                    else
                    {
                        for (var i = 0; i < textureRect.Length; ++i)
                        {
                            var data = new Color[tileWidth * tileHeight];
                            overrideTexture.GetData(0, GetSourceRectForObject(textureRect.Index + i, overrideTexture, tileWidth, tileHeight), data, 0, data.Length);
                            originalTexture.SetData(0, GetSourceRectForObject(spriteOverride.Key + i, originalTexture, tileWidth, tileHeight), data, 0, data.Length);
                        }
                    }
                }
            }
        }

        private void ExtendTexture(ref Texture2D originalTexture, Dictionary<int, TextureRect> spriteOverrides, int tileWidth, int tileHeight)
        {
            var maxKey = spriteOverrides.Keys.Max();
            var maxRect = spriteOverrides[maxKey];
            var maxHeight = GetSourceRectForObject(maxKey, originalTexture, tileWidth, tileHeight).Bottom + (maxRect.Height - 1) * tileHeight;

            if (maxHeight > originalTexture.Height)
            {
                var allData = new Color[originalTexture.Width * originalTexture.Height];
                originalTexture.GetData(allData);

                var newData = new Color[originalTexture.Width * maxHeight];
                Array.Copy(allData, newData, allData.Length);

                originalTexture = new Texture2D(Game1.graphics.GraphicsDevice, originalTexture.Width, maxHeight);
                originalTexture.SetData(newData);
            }
        }

        public static Rectangle GetSourceRectForObject(int index, Texture2D texture, int tileWidth, int tileHeight)
        {
            return GetSourceRectForObject(index, 1, 1, texture, tileWidth, tileHeight);
        }

        public static Rectangle GetSourceRectForObject(int index, int length, int height, Texture2D texture, int tileWidth, int tileHeight)
        {
            return new Rectangle(index % (texture.Width / tileWidth) * tileWidth, index / (texture.Width / tileWidth) * tileHeight, tileWidth * length, tileHeight * height);
        }

        #endregion
    }
}