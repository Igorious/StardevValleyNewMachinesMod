using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Delegates;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Utils;
using StardewValley;
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
            PutItem(GetOutputID(item), GetOutputCount(item), GetOutputQuality(item), GetOutputName(item), GetOutputPrice(item));
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
    }
}
