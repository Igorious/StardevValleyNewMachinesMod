using System.ComponentModel;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public class CraftableInformation : IInformation
    {
        public DynamicID<CraftableID> ID { get; set; }

        public string Name { get; set; }

        public int Price { get; set; }

        [DefaultValue(Object.inedible)]
        public int Edibility { get; set; } = Object.inedible;

        [DefaultValue("Crafting")]
        public string Type { get; set; } = "Crafting";

        [DefaultValue(Object.BigCraftableCategory)]
        public int Category { get; set; } = Object.BigCraftableCategory;

        public string Description { get; set; }

        [DefaultValue(true)]
        public bool CanSetOutdoor { get; set; } = true;

        [DefaultValue(true)]
        public bool CanSetIndoor { get; set; } = true;

        [DefaultValue(Object.fragility_Removable)]
        public int Fragility { get; set; } = Object.fragility_Removable;

        [DefaultValue(1)]
        public int ResourceLength { get; set; } = 1;

        int IInformation.ID => ID;

        public override string ToString()
        {
            return $"{Name}/{Price}/{Edibility}/{Type} {Category}/{Description}/{CanSetOutdoor}/{CanSetIndoor}/{Fragility}";
        }
    }
}