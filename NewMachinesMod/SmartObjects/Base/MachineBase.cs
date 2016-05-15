using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Helpers;
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
            (Output.Sounds ?? DefaultSound).ForEach(PlaySound);
            if (Output.Animation != null) PlayAnimation(farmer, Output.Animation.Value);
            minutesUntilReady = GetMinutesUntilReady(item);
            return true;
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
                InventoryHelper.RemoveItem(and.ID, and.Count, farmer);
                return true;
            }
        }

        #region Config Getters

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

        #endregion
    }
}
