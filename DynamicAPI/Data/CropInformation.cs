using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Igorious.StardewValley.DynamicAPI.Interfaces;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class CropInformation : IDrawable
    {
        public int SeedID { get; set; }
        public List<int> Phases { get; set; } = new List<int>();
        public List<string> Seasons { get; set; } = new List<string>();
        public int TextureIndex { get; set; }
        public int CropID { get; set; }
        [DefaultValue(-1)]
        public int RegrowDays { get; set; } = -1;
        public int HarvestMethod { get; set; } = 0;
        public bool UseAdditionalParameters { get; set; }
        public int? MinHarvest { get; set; }
        public int? MaxHarvest { get; set; }
        public int? MaxHarvestIncreaseForLevel { get; set; }
        public decimal? ExtraCropChance { get; set; }
        public bool IsRaisedSeeds { get; set; }
        public bool UseRandomColors { get; set; }
        public List<int> Colors { get; set; }

        public override string ToString()
        {
            var buffer = new StringBuilder($"{string.Join(" ", Phases)}/{string.Join(" ", Seasons)}"
                + $"/{TextureIndex}/{CropID}/{RegrowDays}/{HarvestMethod}/{UseAdditionalParameters}");
            if (UseAdditionalParameters) buffer.Append($" {MinHarvest} {MaxHarvest} {MaxHarvestIncreaseForLevel} {ExtraCropChance}");
            buffer.Append($"/{IsRaisedSeeds}/{UseRandomColors}");
            if (UseRandomColors) buffer.Append(" ").Append(string.Join(" ", Colors));

            return buffer.ToString();
        }

        public int? ResourceIndex { get; set; }
        [DefaultValue(1)]
        int IDrawable.ResourceLength { get; } = 1;
    }
}
