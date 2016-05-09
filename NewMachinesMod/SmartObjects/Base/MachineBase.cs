using System;
using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Objects;
using Igorious.StardewValley.DynamicAPI.Utils;
using Igorious.StardewValley.NewMachinesMod.Data;
using Microsoft.Xna.Framework;
using StardewValley;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class MachineBase : SmartBigCrafrableBase
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
            if (!ProcessRequiredItems(item, farmer)) return false;

            PutItem(GetOutputID(item), GetOutputCount(item), GetOutputQuality(item), GetOutputName(item), GetOutputPrice(item), GetColor(item));
            PlayDropInSounds();
            PlayDropInAnimation(farmer);
            minutesUntilReady = GetMinutesUntilReady(item);
            return true;
        }

        private void PlayDropInAnimation(Farmer farmer)
        {
            if (Output.Animation == null) return;
            PlayAnimation(farmer, Output.Animation.Value);
        }

        protected bool ProcessRequiredItems(Object item, Farmer farmer)
        {
            var outputItem = GetOutputItem(item);

            if (item.Stack < outputItem.InputCount)
            {
                ShowRedMessage(farmer, $"Requires {outputItem.InputCount} {item.Name}");
                return false;
            }

            var and = outputItem.And;
            if (and == null)
            {
                item.Stack -= (outputItem.InputCount - 1);
                return true;
            }

            var andName = new Object(and.ID, 1).Name;
            var actualCount = farmer.getTallyOfObject(and.ID, false);
            if (actualCount < and.Count)
            {
                ShowRedMessage(farmer, $"Requires {and.Count} {andName}");
                return false;
            }
            else
            {
                item.Stack -= (outputItem.InputCount - 1);
                RemoveItems(and.ID, and.Count, farmer);
                return true;
            }
        }

        private void RemoveItems(int id, int count, Farmer farmer)
        {
            var items = farmer.Items;
            var remainedCount = count;
            for (var i = items.Count - 1; i >= 0; --i)
            {
                if ((items[i] as Object)?.ParentSheetIndex != id) continue;

                var delta = Math.Min(items[i].Stack, remainedCount);
                items[i].Stack -= delta;
                remainedCount -= delta;

                if (items[i].Stack == 0) items[i] = null;
                if (remainedCount == 0) break;
            }
        }

        protected virtual void PlayDropInSounds()
        {
            (Output.Sounds ?? DefaultSound).ForEach(PlaySound);
        }

        private OutputItem GetOutputItem(Object item)
        {
            OutputItem outputInfo;
            return Output.Items.TryGetValue(item.ParentSheetIndex, out outputInfo) ? outputInfo : Output.Items[item.Category];
        }

        protected virtual string GetOutputName(Object item)
        {
            var itemNameFormat = GetOutputItem(item)?.Name;
            if (!string.IsNullOrWhiteSpace(itemNameFormat)) return string.Format(itemNameFormat, "{0}", item.Name);
            if (itemNameFormat != null && string.IsNullOrWhiteSpace(itemNameFormat)) return null;
            return (Output.Name != null) ? string.Format(Output.Name, "{0}", item.Name) : null;
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

        protected virtual int GetMinutesUntilReady(Object item)
        {
            return GetOutputItem(item)?.MinutesUntilReady ?? Output.MinutesUntilReady ?? 0;
        }

        private Color? GetColor(Object dropInItem)
        {
            var color = GetOutputItem(dropInItem)?.Color;
            if (string.IsNullOrWhiteSpace(color)) return null;

            return (color != "@")
                ? RawColor.FromHex(color).ToXnaColor()
                : DominantColorFinder.GetDominantColor(dropInItem.ParentSheetIndex, Game1.objectSpriteSheet, 16, 16);
        }
    }
}
