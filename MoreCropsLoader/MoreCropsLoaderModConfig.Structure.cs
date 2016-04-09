using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI;
using Igorious.StardewValley.DynamicAPI.Data;

namespace Igorious.StardewValley.MoreCropsLoader
{
    public sealed class MoreCropsLoaderModConfig : DynamicConfiguration
    {
        public List<ObjectInformation> Items { get; set; } = new List<ObjectInformation>();
        public List<TreeInformation> Trees { get; set; } = new List<TreeInformation>();
        public List<CropInformation> Crops { get; set; } = new List<CropInformation>();
    }
}
