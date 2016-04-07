using System.Collections.Generic;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class OutputInfo
    {
        public int? ID { get; set; }
        public Dictionary<int, OutputItem> Items { get; set; } = new Dictionary<int, OutputItem>();
        public string Price { get; set; }
        public string Quality { get; set; }
        public string Count { get; set; }
    }
}