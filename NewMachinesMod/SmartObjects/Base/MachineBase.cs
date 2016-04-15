using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Delegates;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class MachineBase : SmartObjectBase
    {
        protected MachineBase(int id) : base(id) { }

        protected abstract IMachineOutput MachineOutput { get; }
        protected OutputInfo Output => MachineOutput.Output;
        protected int? MinutesUntilReady => MachineOutput.MinutesUntilReady;
        private static readonly List<Sound> DefaultSound = new List<Sound> { Sound.Ship };

        protected override bool CanPerformDropIn(Object item, Farmer farmer)
        {
            return (heldObject == null) && Output.Items.ContainsKey(item.ParentSheetIndex);
        }

        protected override bool PerformDropIn(Object item, Farmer farmer)
        {
            PutItem(GetOutputID(item), GetOutputCount(item), GetOutputQuality(item), GetOutputName(item), GetOutputPrice(item), GetColor(item));
            PlayDropInSounds();
            minutesUntilReady = GetMinutesUntilReady(item);
            return true;
        }

        protected virtual void PlayDropInSounds()
        {
            (MachineOutput.Sounds ?? DefaultSound).ForEach(PlaySound);
        }

        protected virtual string GetOutputName(Object item)
        {
            var itemName = Output.Items[item.ParentSheetIndex]?.Name;
            if (!string.IsNullOrWhiteSpace(itemName)) return itemName;
            if (itemName != null && string.IsNullOrWhiteSpace(itemName) || Output.NameFormat == null) return null;
            return string.Format(Output.NameFormat, item.Name);
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

        private Color? GetColor(Object item)
        {
            Log.SyncColour($"Dominant color: {GetDominantColor(item.parentSheetIndex)}", ConsoleColor.Cyan);
            var colorString = Output.Items[item.ParentSheetIndex]?.Color;
            return (colorString != null) ? RawColor.FromHex(colorString).ToXnaColor() : (Color?)null;
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

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            base.draw(spriteBatch, x, y, alpha);
            if (heldObject is ColoredObject)
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

        private string GetDominantColor(int spriteIndex)
        {
            const int width = 16;
            const int height = 16;
            const int minDiversion = 15;

            var rect = Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, spriteIndex, width, height);
            var data = new Color[width * height];
            Game1.objectSpriteSheet.GetData(0, rect, data, 0, data.Length);

            var dicColors = new Dictionary<string, long>();

            for (var y = 0; y < height; y++)
                for (var x = 0; x < width; x++)
                {
                    var color = data[y * width + x];
                    if (color.A == 0) continue;
                    var red = RoundColorToGroup(color.R);
                    var green = RoundColorToGroup(color.G);
                    var blue = RoundColorToGroup(color.B);

                    if (Math.Abs(red - green) > minDiversion || Math.Abs(red - blue) > minDiversion || Math.Abs(green - blue) > minDiversion)
                    {
                        var colorGroup = new RawColor(red, green, blue).ToHex();
                        if (dicColors.ContainsKey(colorGroup))
                        {
                            dicColors[colorGroup]++;
                        }
                        else
                        {
                            dicColors.Add(colorGroup, 1);
                        }
                    }
                }

            return dicColors.OrderByDescending(x => x.Value).First().Key;
        }

        public static int RoundColorToGroup(int i)
        {
            return (i + 4) / 10 * 10;
        }
    }
}
