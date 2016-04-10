using System.ComponentModel;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Extensions;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class WayToGetCookingRecipe
    {
        public string FriendshipWith { get; set; }
        public int? Hearts { get; set; }

        public Skill Skill { get; set; }
        public int? SkillLevel { get; set; }

        public bool FromTV { get; set; }
        [DefaultValue(100)]
        private int FromTVIndex { get; set; } = 100;

        public static WayToGetCookingRecipe Parse(string recipeWayToGet)
        {
            var parts = recipeWayToGet.Split(' ');
            var way = new WayToGetCookingRecipe();
            switch (parts[0])
            {
                case "f":
                    way.FriendshipWith = parts[1];
                    way.Hearts = int.Parse(parts[2]);
                    break;

                case "s":
                    way.Skill = parts[1].ToEnum<Skill>();
                    way.SkillLevel = int.Parse(parts[2]);
                    break;

                case "l":
                    way.FromTV = true;
                    way.FromTVIndex = int.Parse(parts[1]);
                    break;
            }
            return way;
        }

        public override string ToString()
        {
            if (FriendshipWith != null) return $"f {FriendshipWith} {Hearts}";
            if (Skill != Skill.Undefined) return $"s {Skill} {SkillLevel}";
            if (FromTV) return $"l {FromTVIndex}";
            return "default";
        }
    }
}