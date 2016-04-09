using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using StardewModdingAPI.Events;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class CraftingRecipesService
    {
        #region Singleton Access

        private CraftingRecipesService()
        {
            GameEvents.LoadContent += (s, e) => LoadToGame();
            PlayerEvents.LoadedGame += (s, e) => LoadToPlayer();
        }

        private static CraftingRecipesService _instance;

        public static CraftingRecipesService Instance => _instance ?? (_instance = new CraftingRecipesService());

        #endregion

        #region Private Data

        private readonly List<CraftingRecipeInformation> _customRecipeInformations = new List<CraftingRecipeInformation>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Register a custom crafting recipe.
        /// </summary>
        public void Register(int id, string name, IEnumerable<MaterialInfo> materials, string skill, int? skillLevel = 0)
        {
            _customRecipeInformations.Add(new CraftingRecipeInformation {ID = id, Name = name, Materials = materials, Skill = skill, SkillLevel = skillLevel});
        }

        /// <summary>
        /// Register a custom crafting recipe.
        /// </summary>
        public void Register(ICraftable craftableItem)
        {
            Register(craftableItem.ID, 
                craftableItem.Name, 
                craftableItem.Materials.Select(kv => new MaterialInfo(kv.Key, kv.Value)), 
                craftableItem.Skill, 
                craftableItem.SkillLevel);
        }

        #endregion

        #region	Auxiliary Methods

        private void LoadToGame()
        {
            foreach (var recipeInformation in _customRecipeInformations)
            {
                if (CraftingRecipe.craftingRecipes.ContainsKey(recipeInformation.Name)) continue;
                CraftingRecipe.craftingRecipes.Add(recipeInformation.Name, recipeInformation.ToString());
            }
        }

        private void LoadToPlayer()
        {
            var player = Game1.player;
            foreach (var recipeInformation in _customRecipeInformations)
            {
                if (player.craftingRecipes.ContainsKey(recipeInformation.Name)) continue;
                if (recipeInformation.Skill == null
                    || recipeInformation.Skill == Skill.Farming && player.FarmingLevel >= recipeInformation.SkillLevel)
                {
                    player.craftingRecipes.Add(recipeInformation.Name, 0);
                }
            }
        }

        #endregion
    }
}