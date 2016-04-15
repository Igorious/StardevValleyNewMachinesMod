using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Extensions;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public class CraftingRecipeInformation
    {
        public string Name { get; set; }

        public List<IngredientInfo> Materials { get; set; } = new List<IngredientInfo>();

        [DefaultValue("Home")]
        private string Area { get; set; } = "Home";

        public DynamicID<CraftableID> ID { get; set; }

        public bool IsBig { get; set; } = true;

        public WayToGetCraftingRecipe WayToGet { get; set; } = new WayToGetCraftingRecipe();

        public static CraftingRecipeInformation Parse(string craftingRecipeInformation)
        {
            var parts = craftingRecipeInformation.Split('/');
            var info = new CraftingRecipeInformation();
            var materials = parts[0].Split(' ').Select(int.Parse).ToList();
            for (var i = 0; i < materials.Count; i += 2)
            {
                info.Materials.Add(new IngredientInfo(materials[i], materials[i + 1]));
            }
            info.Area = parts[1];
            info.ID = int.Parse(parts[2]);
            info.IsBig = bool.Parse(parts[3]);
            info.WayToGet = WayToGetCraftingRecipe.Parse(parts[4]);
            return info;
        }

        public override string ToString()
        {
            return $"{string.Join(" ", Materials)}/{Area}/{ID}/{IsBig.ToLower()}/{WayToGet}";
        }
    }
}