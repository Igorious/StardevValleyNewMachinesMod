using System;
using Igorious.StardewValley.DynamicAPI;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Utils;
using StardewValley;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.NewMachinesMod.SmartObjects.Base
{
    public abstract class MachineBase : SmartObjectBase
    {
        protected MachineBase(int id) : base(id) {}

        protected abstract OutputInfo Output { get; }
        protected abstract int MinutesUntilReady { get; }

        protected override bool CanPerformDropIn(Object item, Farmer farmer)
        {
            return (heldObject == null) && Output.Items.ContainsKey(item.ParentSheetIndex);
        }

        protected override void PerformDropIn(Object item, Farmer farmer)
        {
            PutItem(GetOutputID(item), GetOutputCount(item), GetOutputQuality(item), GetOutputName(item), GetOutputPrice(item));
            PlayDropInSounds();         
            minutesUntilReady = MinutesUntilReady;
        }

        protected virtual void PlayDropInSounds()
        {
            PlaySound(Sound.Ship);
        }

        public static Func<int, int, double, double, int> GetCustomQualityFunc(string body)
        {
            int value;
            if (int.TryParse(body, out value)) return (p, q, r1, r2) => value;
            return ExpressionCompiler.CompileExpression<Func<int, int, double, double, int>>(body, "p", "q", "r1", "r2");
        }

        public static Func<int, int, double, double, int> GetCustomCountFunc(string body)
        {
            int value;
            if (int.TryParse(body, out value)) return (p, q, r1, r2) => value;
            return ExpressionCompiler.CompileExpression<Func<int, int, double, double, int>>(body, "p", "q", "r1", "r2");
        }

        public static Func<int, int, int> GetCustomPriceFunc(string body)
        {
            int value;
            if (int.TryParse(body, out value)) return (p, q) => value;
            return ExpressionCompiler.CompileExpression<Func<int, int, int>>(body, "p", "q");
        }

        protected string GetOutputName(Object item)
        {
            return Output.Items[item.ParentSheetIndex]?.Name;
        }

        protected int GetOutputQuality(Object item)
        {
            var customQualityFunc = GetCustomQualityFunc(Output.Items[item.ParentSheetIndex]?.Quality ?? Output.Quality);
            if (customQualityFunc == null) return 0;
            var random = GetRandom();
            return customQualityFunc(item.Price, item.quality, random.NextDouble(), random.NextDouble());
        }

        protected int GetOutputCount(Object item)
        {
            var customCountFunc = GetCustomCountFunc(Output.Items[item.ParentSheetIndex]?.Count ?? Output.Count);
            if (customCountFunc == null) return 1;
            var random = GetRandom();
            return customCountFunc(item.Price, item.quality, random.NextDouble(), random.NextDouble());
        }

        protected int? GetOutputPrice(Object item)
        {
            return Output.Items[item.ParentSheetIndex]?.Price 
                ?? GetCustomPriceFunc(Output.Price)?.Invoke(item.price, item.quality);
        }

        protected int GetOutputID(Object item)
        {
            return Output.Items[item.ParentSheetIndex]?.ID ?? Output.ID ?? 0;
        }
    }
}
