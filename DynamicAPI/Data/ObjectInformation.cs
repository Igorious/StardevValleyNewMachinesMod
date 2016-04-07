using System.Text;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public class ObjectInformation
    {
        public static readonly int[] NoSkillUps = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0};

        public int ID { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; } = 0;
        public int Edibility { get; set; } = Object.inedible;
        public string Type { get; set; } = "Crafting";
        public int Category { get; set; } = Object.BigCraftableCategory;
        public string Description { get; set; } = string.Empty;
        public string Subcategory { get; set; }
        public int[] SkillUps { get; set; }
        public int? Duration { get; set; }

        public override string ToString()
        {
            var buffer = new StringBuilder($"{Name}/{Price}/{Edibility}/{Type} {Category}/{Description}");
            if (Subcategory != null)
            {
                buffer.Append('/').Append(Subcategory)
                    .Append('/').Append(string.Join(" ", SkillUps ?? NoSkillUps))
                    .Append('/').Append(Duration ?? 0);
            }
            return buffer.ToString();
        }
    }
}