using System.Collections.Generic;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public class CraftingRecipeInformation
    {
        public IEnumerable<MaterialInfo> Materials { get; set; } = new MaterialInfo[] {};
        public string Area { get; set; } = "Home";
        public int ID { get; set; } = 0;
        public bool IsBig { get; set; } = true;
        public string Skill { get; set; } = null;
        public int? SkillLevel { get; set; } = 0;
        public string Name { get; set; } = string.Empty;

        public override string ToString()
        {
            var requirements = (Skill == null)? "null" : $"{Skill} {SkillLevel}";
            return $"{string.Join(" ", Materials)}/{Area}/{ID}/{IsBig}/{requirements}";
        }
    }
}