using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Attributes;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Delegates;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class OutputInfo
    {
        public DynamicID<ItemID> ID { get; set; }

        public Dictionary<DynamicID<ItemID>, OutputItem> Items { get; set; } = new Dictionary<DynamicID<ItemID>, OutputItem>();

        [Expression(typeof(PriceExpression))] 
        public string Price { get; set; }

        [Expression(typeof(QualityExpression))] 
        public string Quality { get; set; }

        [Expression(typeof(CountExpression))] 
        public string Count { get; set; }

        public string NameFormat { get; set; }
    }
}