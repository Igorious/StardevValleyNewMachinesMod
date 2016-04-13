using System;
using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Delegates;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using Color = Microsoft.Xna.Framework.Color;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class MachineBase : SmartObjectBase
    {
        protected MachineBase(int id) : base(id) {}

        protected abstract IMachineOutput MachineOutput { get; }
        protected OutputInfo Output => MachineOutput.Output;
        protected int? MinutesUntilReady => MachineOutput.MinutesUntilReady;

        protected override bool CanPerformDropIn(Object item, Farmer farmer)
        {
            return (heldObject == null) && Output.Items.ContainsKey(item.ParentSheetIndex);
        }

        protected override bool PerformDropIn(Object item, Farmer farmer)
        {
            PutItem(GetOutputID(item), GetOutputCount(item), GetOutputQuality(item), GetOutputName(item), GetOutputPrice(item), GetOutputColor(item));
            PlayDropInSounds();         
            minutesUntilReady = GetMinutesUntilReady(item);
            return true;
        }

        protected virtual void PlayDropInSounds()
        {
            (MachineOutput.Sounds ?? new List<Sound> {Sound.Ship}).ForEach(PlaySound);
        }

        protected virtual string GetOutputName(Object item)
        {
            return Output.Items[item.ParentSheetIndex]?.Name;
        }

        private Color? GetOutputColor(Object item)
        {
            var colorString = Output.Items[item.ParentSheetIndex]?.Color;
            return DynamicAPI.Data.Color.FromHex(colorString);
        }

        protected virtual int GetOutputQuality(Object item)
        {
            var qualityExpression = Output.Items[item.ParentSheetIndex]?.Quality ?? Output.Quality;
            var calculateQuality = ExpressionCompiler.CompileExpression<QualityExpression>(qualityExpression);
            if (calculateQuality == null) return 0;

            var random = GetRandom();
            return calculateQuality(item.Price, item.quality, random.NextDouble(), random.NextDouble());
        }

        protected virtual int GetOutputCount(Object item)
        {
            var countExpression = Output.Items[item.ParentSheetIndex]?.Count ?? Output.Count;
            var calculateCount = ExpressionCompiler.CompileExpression<CountExpression>(countExpression);
            if (calculateCount == null) return 1;

            var random = GetRandom();
            return calculateCount(item.Price, item.quality, random.NextDouble(), random.NextDouble());
        }

        protected virtual int? GetOutputPrice(Object item)
        {
            var itemPrice = Output.Items[item.ParentSheetIndex]?.Price;
            if (itemPrice != null) return itemPrice;

            var calculatePrice = ExpressionCompiler.CompileExpression<PriceExpression>(Output.Price);
            return calculatePrice?.Invoke(item.price, item.quality);
        }

        protected virtual int GetOutputID(Object item)
        {
            return Output.Items[item.ParentSheetIndex]?.ID ?? Output.ID ?? 0;
        }

        protected virtual int GetMinutesUntilReady(Object item)
        {
            return Output.Items[item.ParentSheetIndex]?.MinutesUntilReady ?? MinutesUntilReady ?? 0;
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

        protected void DrawSprite(int spriteIndex, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            var v1 = getScale() * Game1.pixelZoom;
            var v2 = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize, y * Game1.tileSize - Game1.tileSize));
            var destinationX = v2.X - v1.X / 2 + (shakeTimer > 0? Game1.random.Next(-1, 2) : 0);
            var destinationY = v2.Y - v1.Y / 2 + (shakeTimer > 0? Game1.random.Next(-1, 2) : 0);
            var width = Game1.tileSize + v1.X;
            var height = Game1.tileSize * 2 + v1.Y / 2;
            var destinationRectangle = new Rectangle((int)destinationX, (int)destinationY, (int)width, (int)height);
            spriteBatch.Draw(Game1.bigCraftableSpriteSheet, destinationRectangle, getSourceRectForBigCraftable(spriteIndex), Microsoft.Xna.Framework.Color.White * alpha, 0, Vector2.Zero, SpriteEffects.None, Math.Max(0, ((y + 1) * Game1.tileSize - Game1.pixelZoom * 6) / 10000f));
            if (!readyForHarvest) return;

            var num = (float)(4.0 * Math.Round(Math.Sin(DateTime.Now.TimeOfDay.TotalMilliseconds / 250), 2));
            var depth = (float)((y + 1) * Game1.tileSize / 10000.0 + tileLocation.X / 10000.0);
            spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize - 8, y * Game1.tileSize - Game1.tileSize * 3 / 2 - 16 + num)), new Rectangle(141, 465, 20, 24), Microsoft.Xna.Framework.Color.White * 0.75f, 0, Vector2.Zero, 4, SpriteEffects.None, depth - 0.001f);
            spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * Game1.tileSize + Game1.tileSize / 2, y * Game1.tileSize - Game1.tileSize - Game1.tileSize / 8 + num)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, heldObject.parentSheetIndex, 16, 16), Microsoft.Xna.Framework.Color.White * 0.75f, 0, new Vector2(8, 8), Game1.pixelZoom, SpriteEffects.None, depth);
        }
    }
}
