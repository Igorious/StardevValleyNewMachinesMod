using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class CropInformation : IDrawable, ICropInformation
    {
        public int SeedID { get; set; }

        public List<int> Phases { get; set; } = new List<int>();

        public List<Season> Seasons { get; set; } = new List<Season>();

        [DefaultValue(-1)]
        public int TextureIndex { get; set; } = -1;

        public int CropID { get; set; }

        [DefaultValue(-1)]
        public int RegrowDays { get; set; } = -1;

        public int HarvestMethod { get; set; }

        [JsonIgnore]
        public bool UseAdditionalParameters => (MinHarvest != 1) || (MaxHarvest != 1) || (MaxHarvestIncreaseForLevel != 0) || (ExtraCropChance != 0);

        [DefaultValue(1)]
        public int MinHarvest { get; set; } = 1;

        [DefaultValue(1)]
        public int MaxHarvest { get; set; } = 1;

        [DefaultValue(0)]
        public int MaxHarvestIncreaseForLevel { get; set; }

        [DefaultValue(0)]
        public decimal ExtraCropChance { get; set; }

        public bool IsRaisedSeeds { get; set; }

        [JsonIgnore]
        public bool UseRandomColors => (Colors?.Count > 0);

        public List<string> Colors { get; set; }

        public int? ResourceIndex { get; set; }

        [DefaultValue(1)]
        public int ResourceLength { get; set; } = 1;

        int IInformation.ID => SeedID;

        public static CropInformation Parse(string cropInformation)
        {
            var info = new CropInformation();
            var parts = cropInformation.Split('/');
            info.Phases = parts[0].Split(' ').Select(int.Parse).ToList();
            info.Seasons = parts[1].Split(' ').Select(s => s.ToEnum<Season>()).ToList();
            info.TextureIndex = int.Parse(parts[2]);
            info.CropID = int.Parse(parts[3]);
            info.RegrowDays = int.Parse(parts[4]);
            info.HarvestMethod = int.Parse(parts[5]);
            var additionalParameters = parts[6].Split(' ');
            if (additionalParameters.Length > 1)
            {
                info.MinHarvest = int.Parse(additionalParameters[1]);
                info.MaxHarvest = int.Parse(additionalParameters[2]);
                info.MaxHarvestIncreaseForLevel = int.Parse(additionalParameters[3]);
                info.ExtraCropChance = decimal.Parse(additionalParameters[4]);
            }
            info.IsRaisedSeeds = bool.Parse(parts[7]);
            var colors = parts[8].Split(' ');
            if (colors.Length > 1)
            {
                var intColors = colors.Skip(1).Select(int.Parse).ToList();
                info.Colors = new List<string>();
                for (var i = 0; i < intColors.Count; i += 3)
                {
                    info.Colors.Add(new RawColor(intColors[i], intColors[i + 1], intColors[i + 2]).ToHex());
                }
            }
            return info;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder($"{string.Join(" ", Phases)}/{string.Join(" ", Seasons.Select(s => s.ToLower()))}"
                + $"/{TextureIndex}/{CropID}/{RegrowDays}/{HarvestMethod}/{UseAdditionalParameters.ToLower()}");
            if (UseAdditionalParameters)
            {
                buffer.Append($" {MinHarvest} {MaxHarvest} {MaxHarvestIncreaseForLevel} ");
                buffer.Append(ExtraCropChance == 0 ? "0" : $"{ExtraCropChance:.#####}");
            }
            buffer.Append($"/{IsRaisedSeeds.ToLower()}/{UseRandomColors.ToLower()}");
            if (UseRandomColors)
            {
                buffer.Append(" ").Append(string.Join(" ", Colors.Select(RawColor.FromHex)));
            }

            return buffer.ToString();
        }
    }
}
