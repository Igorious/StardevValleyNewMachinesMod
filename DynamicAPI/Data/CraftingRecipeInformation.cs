using System.Collections.Generic;
using System.ComponentModel;
using Igorious.StardewValley.DynamicAPI.Interfaces;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public class CraftingRecipeInformation : IInformation
    {
        public IEnumerable<MaterialInfo> Materials { get; set; } = new MaterialInfo[] {};

        [DefaultValue("Home")]
        public string Area { get; set; } = "Home";
        public int ID { get; set; }
        public bool IsBig { get; set; } = true;
        public string Skill { get; set; } = null;
        public int? SkillLevel { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            var requirements = (Skill == null)? "null" : $"{Skill} {SkillLevel}";
            return $"{string.Join(" ", Materials)}/{Area}/{ID}/{IsBig}/{requirements}";
        }
    }
}