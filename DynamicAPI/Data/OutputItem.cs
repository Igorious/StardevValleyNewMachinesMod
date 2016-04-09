using Igorious.StardewValley.DynamicAPI.Attributes;
using Igorious.StardewValley.DynamicAPI.Delegates;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class OutputItem
    {
        public int? ID { get; set; }

        public string Name { get; set; }

        public int? Price { get; set; }

        [Expression(typeof(QualityExpression))] 
        public string Quality { get; set; }

        [Expression(typeof(CountExpression))] 
        public string Count { get; set; }

        public int? MinutesUntilReady { get; set; }
    }
}