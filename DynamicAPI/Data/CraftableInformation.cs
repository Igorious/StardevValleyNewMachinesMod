using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public class CraftableInformation
    {
        public int ID { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; } = 0;
        public int Edibility { get; set; } = Object.inedible;
        public string Type { get; set; } = "Crafting";
        public int Category { get; set; } = Object.BigCraftableCategory;
        public string Description { get; set; } = string.Empty;
        public bool CanSetOutdoor { get; set; } = true;
        public bool CanSetIndoor { get; set; } = true;
        public int Fragility { get; set; } = Object.fragility_Removable;

        public override string ToString()
        {
            return $"{Name}/{Price}/{Edibility}/{Type} {Category}/{Description}/{CanSetOutdoor}/{CanSetIndoor}/{Fragility}";
        }
    }
}