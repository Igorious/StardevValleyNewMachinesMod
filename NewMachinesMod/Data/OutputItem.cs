using Igorious.StardewValley.DynamicAPI.Attributes;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Newtonsoft.Json;

namespace Igorious.StardewValley.NewMachinesMod.Data
{
    public sealed class OutputItem
    {
        #region	Constructors

        [JsonConstructor]
        public OutputItem() { }

        public OutputItem(DynamicID<ItemID> id, string name = null)
        {
            ID = id;
            Name = name;
        }

        #endregion

        #region	Properties

        /// <summary>
        /// ID of output item.
        /// </summary>
        [JsonProperty]
        public DynamicID<ItemID> ID { get; set; }

        /// <summary>
        /// Name format of output item. Use {0} to insert default name of output item, {1} to insert name of input item.
        /// </summary>
        [JsonProperty]
        public string Name { get; set; }

        /// <summary>
        /// Specified price of output item. If it's <c>null</c>, default value will be used.
        /// </summary>
        [JsonProperty, Expression(typeof(PriceExpression))] 
        public string Price { get; set; }

        /// <summary>
        /// Specified quality of output item. If it's <c>null</c>, default value will be used.
        /// </summary>
        [JsonProperty, Expression(typeof(QualityExpression))] 
        public string Quality { get; set; }

        /// <summary>
        /// Specified count of output item. If it's <c>null</c>, default value will be used.
        /// </summary>
        [JsonProperty, Expression(typeof(CountExpression))] 
        public string Count { get; set; }

        /// <summary>
        /// Specified time of output item processing. If it's <c>null</c>, default value will be used.
        /// </summary>
        [JsonProperty]
        public int? MinutesUntilReady { get; set; }

        /// <summary>
        /// Color of output item. If it's <c>null</c>, color will not be used. 
        /// If it's <c>"@"</c>, auto-generated value will be used.
        /// </summary>
        [JsonProperty]
        public string Color { get; set; }

        #endregion
    }
}