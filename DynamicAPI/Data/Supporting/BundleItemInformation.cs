using System.ComponentModel;
using Igorious.StardewValley.DynamicAPI.Constants;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Data.Supporting
{
    public sealed class BundleItemInformation
    {
        #region	Constructors

        [JsonConstructor]
        public BundleItemInformation() { }

        public BundleItemInformation(DynamicID<ItemID> id, int count = 1, int quality = 1)
        {
            ID = id;
            Count = count;
            Quality = quality;
        }

        #endregion

        #region Properties

        [JsonProperty(Required = Required.Always)]
        public DynamicID<ItemID> ID { get; set; }

        [JsonProperty, DefaultValue(1)]
        public int Count { get; set; } = 1;

        [JsonProperty]
        public int Quality { get; set; }

        #endregion

        public override string ToString()
        {
            return $"{ID} {Count} {Quality}";
        }
    }
}