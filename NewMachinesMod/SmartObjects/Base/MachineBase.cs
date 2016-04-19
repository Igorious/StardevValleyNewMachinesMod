using System;
using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Objects;
using Igorious.StardewValley.DynamicAPI.Utils;
using Igorious.StardewValley.NewMachinesMod.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class MachineBase : SmartObjectBase
    {
        protected MachineBase(int id) : base(id) { }

        protected abstract MachineOutputInformation Output { get; }
        private static readonly List<Sound> DefaultSound = new List<Sound> { Sound.Ship };

        protected override bool CanPerformDropIn(Object item, Farmer farmer)
        {
            return (heldObject == null) && (Output.Items.ContainsKey(item.ParentSheetIndex) || Output.Items.ContainsKey(item.Category));
        }

        protected override bool PerformDropIn(Object item, Farmer farmer)
        {
            var color = GetColor(item);
            PutItem(GetOutputID(item), GetOutputCount(item), GetOutputQuality(item), GetOutputName(item), GetOutputPrice(item), color != null);
            SetColor(item, color);
            PlayDropInSounds();
            minutesUntilReady = GetMinutesUntilReady(item);
            return true;
        }

        protected virtual void PlayDropInSounds()
        {
            (Output.Sounds ?? DefaultSound).ForEach(PlaySound);
        }

        private OutputItem GetOutputItem(Object item)
        {
            OutputItem outputInfo;
            return Output.Items.TryGetValue(item.ParentSheetIndex, out outputInfo)? outputInfo : Output.Items[item.Category];
        }

        protected virtual string GetOutputName(Object item)
        {
            var itemNameFormat = GetOutputItem(item)?.Name;
            if (!string.IsNullOrWhiteSpace(itemNameFormat)) return string.Format(itemNameFormat, "{0}", item.Name);
            if (itemNameFormat != null && string.IsNullOrWhiteSpace(itemNameFormat)) return null;
            return (Output.Name != null)? string.Format(Output.Name, "{0}", item.Name) : null;
        }

        protected virtual int GetOutputQuality(Object item)
        {
            var qualityExpression = GetOutputItem(item)?.Quality ?? Output.Quality;
            var calculateQuality = ExpressionCompiler.CompileExpression<QualityExpression>(qualityExpression);
            if (calculateQuality == null) return 0;

            var random = GetRandom();
            return calculateQuality(item.Price, item.quality, random.NextDouble(), random.NextDouble());
        }

        protected virtual int GetOutputCount(Object item)
        {
            var countExpression = GetOutputItem(item)?.Count ?? Output.Count;
            var calculateCount = ExpressionCompiler.CompileExpression<CountExpression>(countExpression);
            if (calculateCount == null) return 1;

            var random = GetRandom();
            return calculateCount(item.Price, item.quality, random.NextDouble(), random.NextDouble());
        }

        protected virtual int? GetOutputPrice(Object item)
        {
            var priceExpression = GetOutputItem(item)?.Price ?? Output.Price;
            var calculatePrice = ExpressionCompiler.CompileExpression<PriceExpression>(priceExpression);
            return calculatePrice?.Invoke(item.price, item.quality);
        }

        protected virtual int GetOutputID(Object item)
        {
            return GetOutputItem(item)?.ID ?? Output.ID ?? 0;
        }

        private string GetColor(Object item)
        {
            return GetOutputItem(item)?.Color;
        }

        protected virtual int GetMinutesUntilReady(Object item)
        {
            return GetOutputItem(item)?.MinutesUntilReady ?? Output.MinutesUntilReady ?? 0;
        }

        public MachineState State
        {
            get
            {
                if (readyForHarvest) return MachineState.Ready;
                if (heldObject != null) return MachineState.Processing;
                return MachineState.Empty;
            }
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            base.draw(spriteBatch, x, y, alpha);
            if (readyForHarvest && heldObject is ColoredObject)
            {
                var num = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250), 2));
                var depth = (float)((y + 1) * Game1.tileSize / 10000.0 + tileLocation.X / 10000.0);
                spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize + Game1.tileSize / 2, y * Game1.tileSize - Game1.tileSize - Game1.tileSize / 8 + num)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, heldObject.parentSheetIndex + 1, 16, 16), ((ColoredObject)heldObject).color, 0, new Vector2(8, 8), Game1.pixelZoom, SpriteEffects.None, depth + 0.001f);
            }
        }

        protected void DrawSprite(int spriteIndex, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            var v1 = getScale() * Game1.pixelZoom;
            var v2 = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize, y * Game1.tileSize - Game1.tileSize));
            var destinationX = v2.X - v1.X / 2 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0);
            var destinationY = v2.Y - v1.Y / 2 + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0);
            var width = Game1.tileSize + v1.X;
            var height = Game1.tileSize * 2 + v1.Y / 2;
            var destinationRectangle = new Rectangle((int)destinationX, (int)destinationY, (int)width, (int)height);
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, getSourceRectForBigCraftable(spriteIndex), Color.White * alpha, 0, Vector2.Zero, SpriteEffects.None, Math.Max(0, ((y + 1) * Game1.tileSize - Game1.pixelZoom * 6) / 10000f));
            if (!readyForHarvest) return;

            var num = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250), 2));
            var depth = (float)((y + 1) * Game1.tileSize / 10000.0 + tileLocation.X / 10000.0);
            spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize - 8, y * Game1.tileSize - Game1.tileSize * 3 / 2 - 16 + num)), new Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0, Vector2.Zero, 4, SpriteEffects.None, depth - 0.002f);
            spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize + Game1.tileSize / 2, y * Game1.tileSize - Game1.tileSize - Game1.tileSize / 8 + num)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, heldObject.parentSheetIndex, 16, 16), Color.White * 0.75f, 0, new Vector2(8, 8), Game1.pixelZoom, SpriteEffects.None, depth - 0.001f);
            if (heldObject is ColoredObject)
            {
                spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize + Game1.tileSize / 2, y * Game1.tileSize - Game1.tileSize - Game1.tileSize / 8 + num)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, heldObject.parentSheetIndex + 1, 16, 16), ((ColoredObject)heldObject).color, 0, new Vector2(8, 8), Game1.pixelZoom, SpriteEffects.None, depth);
            }
        }      
        
        private void SetColor(Object dropInItem, string color)
        {
            if (string.IsNullOrWhiteSpace(color) || !(heldObject is ColoredObject)) return;

            var coloredItem = (ColoredObject)heldObject;
            coloredItem.color = (color != "@")
                ? RawColor.FromHex(color).ToXnaColor() 
                : DominantColorFinder.GetDominantColor(dropInItem.ParentSheetIndex, Game1.objectSpriteSheet, 16, 16);
        }
    }
}
