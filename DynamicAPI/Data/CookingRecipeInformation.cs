using System.Collections.Generic;
using System.Linq;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class CookingRecipeInformation
    {
        public string Name { get; set; }

        public List<IngredientInfo> Ingredients { get; set; } = new List<IngredientInfo>();

        private string UnknownVar { get; set; } = "1 10";

        public int ID { get; set; }

        public WayToGetCookingRecipe WayToGet { get; set; } = new WayToGetCookingRecipe();

        public static CookingRecipeInformation Parse(string cookingRecipeInformation)
        {
            var parts = cookingRecipeInformation.Split('/');
            var info = new CookingRecipeInformation();
            var ingredients = parts[0].Split(' ').Select(int.Parse).ToList();
            for (var i = 0; i < ingredients.Count; i += 2)
            {
                info.Ingredients.Add(new IngredientInfo(ingredients[i], ingredients[i + 1]));
            }
            info.UnknownVar = parts[1];
            info.ID = int.Parse(parts[2]);
            info.WayToGet = WayToGetCookingRecipe.Parse(parts[3]);
            return info;
        }

        public override string ToString()
        {
            return $"{string.Join(" ", Ingredients)}/{UnknownVar}/{ID}/{WayToGet}";
        }
    }
}