using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class ItemInformation : IItem, IItemInformation
    {
        public static readonly int[] NoSkillUps = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public DynamicID<ItemID> ID { get; set; }

        int IItem.ID => ID;

        public string Name { get; set; }

        int IDrawable.TextureIndex => ID;

        public int Price { get; set; }

        [DefaultValue(Object.inedible)]
        public int Edibility { get; set; } = Object.inedible;

        [DefaultValue(ObjectType.Basic)]
        public ObjectType Type { get; set; } = ObjectType.Basic;

        public ObjectCategory Category { get; set; }

        public string Description { get; set; }

        public MealCategory MealCategory { get; set; }

        public SkillUpInformation SkillUps { get; set; }

        public int? Duration { get; set; }

        public List<DayTime> FishDayTime { get; set; }

        public List<Season> FishSeason { get; set; }

        public List<ArchChance> ArchChances { get; set; }

        public string ArchAdditionalInfo { get; set; }

        public int? ResourceIndex { get; set; }

        [DefaultValue(1)]
        public int ResourceLength { get; set; } = 1;

        public static ItemInformation Parse(string objectInformation)
        {
            var info = new ItemInformation();
            var parts = objectInformation.Split('/');
            info.Name = parts[0];
            info.Price = int.Parse(parts[1]);
            info.Edibility = int.Parse(parts[2]);
            var typeAndCategory = parts[3].Split(' ');
            info.Type = typeAndCategory[0].ToEnum<ObjectType>();
            if (typeAndCategory.Length > 1) info.Category = typeAndCategory[1].ToEnum<ObjectCategory>();
            info.Description = parts[4];
            if (parts.Length > 5)
            {
                if (info.Category == ObjectCategory.Fish)
                {
                    var dayTimeAndSeasons = parts[5].Split('^');
                    info.FishDayTime = dayTimeAndSeasons[0].Split(' ').Select(d => d.ToEnum<DayTime>()).ToList();
                    info.FishSeason = dayTimeAndSeasons[1].Split(' ').Select(d => d.ToEnum<Season>()).ToList();
                }
                else if (info.Type == ObjectType.Arch)
                {
                    var archChances = parts[5].Split(' ');
                    info.ArchChances = new List<ArchChance>();
                    for (var i = 0; i < archChances.Length; i += 2)
                    {
                        info.ArchChances.Add(new ArchChance
                        {
                            Location = archChances[i],
                            Chance = decimal.Parse(archChances[i + 1]),
                        });
                    }
                    info.ArchAdditionalInfo = parts[6];
                }
                else
                {
                    info.MealCategory = parts[5].ToEnum<MealCategory>();
                    info.SkillUps = SkillUpInformation.Parse(parts[6]);
                    info.Duration = int.Parse(parts[7]);
                }
            }
            return info;
        }

        int IInformation.ID => ID;

        public override string ToString()
        {
            var buffer = new StringBuilder($"{Name}/{Price}/{Edibility}/{Type}");
            if (Category != ObjectCategory.Undefined) buffer.Append(' ').Append((int)Category);
            buffer.Append('/').Append(Description);
            if (Category == ObjectCategory.Fish)
            {
                buffer.Append('/').Append(string.Join(" ", FishDayTime))
                    .Append('^').Append(string.Join(" ", FishSeason));
            }
            else if (Type == ObjectType.Arch)
            {
                buffer.Append('/').Append(string.Join(" ", ArchChances))
                    .Append('/').Append(ArchAdditionalInfo);
            }
            else if (MealCategory != MealCategory.Undefined)
            {
                buffer.Append('/').Append(MealCategory.ToLower())
                    .Append('/').Append(SkillUps ?? new SkillUpInformation())
                    .Append('/').Append(Duration ?? 0);
            }
            return buffer.ToString();
        }
    }
}