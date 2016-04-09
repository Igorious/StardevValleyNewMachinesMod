using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Attributes;
using Igorious.StardewValley.DynamicAPI.Delegates;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class OutputInfo
    {
        public int? ID { get; set; }

        public Dictionary<int, OutputItem> Items { get; set; } = new Dictionary<int, OutputItem>();

        [Expression(typeof(PriceExpression))] 
        public string Price { get; set; }

        [Expression(typeof(QualityExpression))] 
        public string Quality { get; set; }

        [Expression(typeof(CountExpression))] 
        public string Count { get; set; }
    }
}