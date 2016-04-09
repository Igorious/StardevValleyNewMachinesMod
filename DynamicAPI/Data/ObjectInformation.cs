using System.ComponentModel;
using System.Text;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class ObjectInformation : IItem, IInformation
    {
        public static readonly int[] NoSkillUps = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public int ID { get; set; }

        public string Name { get; set; }

        int IDrawable.TextureIndex => ID;

        public int Price { get; set; }

        [DefaultValue(Object.inedible)]
        public int Edibility { get; set; } = Object.inedible;

        [DefaultValue("Basic")]
        public string Type { get; set; } = "Basic";

        public ObjectCategory Category { get; set; }

        public string Description { get; set; }

        public string Subcategory { get; set; }

        public SkillUpInformation SkillUps { get; set; }

        public int? Duration { get; set; }

        public int? ResourceIndex { get; set; }

        [DefaultValue(1)]
        public int ResourceLength { get; set; } = 1;

        public static ObjectInformation Parse(string objectInformation)
        {
            var info = new ObjectInformation();
            var parts = objectInformation.Split('/');
            info.Name = parts[0];
            info.Price = int.Parse(parts[1]);
            info.Edibility = int.Parse(parts[2]);
            var typeAndCategory = parts[3].Split(' ');
            info.Type = typeAndCategory[0];
            if (typeAndCategory.Length > 1) info.Category = (ObjectCategory)int.Parse(typeAndCategory[1]);
            info.Description = parts[4];
            if (parts.Length > 5)
            {
                info.Subcategory = parts[5];
                if (info.Subcategory == "drink" || info.Subcategory == "food")
                {
                    info.SkillUps = SkillUpInformation.Parse(parts[6]);
                    info.Duration = int.Parse(parts[7]);
                }
            }
            return info;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder($"{Name}/{Price}/{Edibility}/{Type} {(int)Category}/{Description}");
            if (Subcategory != null)
            {
                buffer.Append('/').Append(Subcategory)
                    .Append('/').Append(SkillUps ?? new SkillUpInformation())
                    .Append('/').Append(Duration ?? 0);
            }
            return buffer.ToString();
        }
    }
}