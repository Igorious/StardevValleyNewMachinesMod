using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using StardewModdingAPI.Events;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public static class CustomCraftingRecipes
    {
        #region Private Data

        private static readonly List<CraftingRecipeInformation> _customRecipeInformations = new List<CraftingRecipeInformation>();
        private static bool IsInitialized { get; set; }

        #endregion

        #region	Public Properties

        public static IReadOnlyList<CraftingRecipeInformation> CustomRecipeInformations => _customRecipeInformations;

        #endregion

        #region Public Methods

        /// <summary>
        /// Register a custom crafting recipe.
        /// </summary>
        public static void Add(int id, string name, IEnumerable<MaterialInfo> materials, string skill, int? skillLevel = 0)
        {
            Initialize();
            _customRecipeInformations.Add(new CraftingRecipeInformation {ID = id, Name = name, Materials = materials, Skill = skill, SkillLevel = skillLevel});
        }

        /// <summary>
        /// Register a custom crafting recipe.
        /// </summary>
        public static void Add(ICraftable craftableItem)
        {
            Add(craftableItem.ID, 
                craftableItem.Name, 
                craftableItem.Materials.Select(kv => new MaterialInfo(kv.Key, kv.Value)), 
                craftableItem.Skill, 
                craftableItem.SkillLevel);
        }

        #endregion

        #region	Auxiliary Methods

        private static void Initialize()
        {
            if (IsInitialized) return;
            GameEvents.LoadContent += (s, e) => LoadToGame();
            PlayerEvents.LoadedGame += (s, e) => LoadToPlayer();
            IsInitialized = true;
        }

        private static void LoadToGame()
        {
            foreach (var recipeInformation in CustomRecipeInformations)
            {
                if (CraftingRecipe.craftingRecipes.ContainsKey(recipeInformation.Name)) continue;
                CraftingRecipe.craftingRecipes.Add(recipeInformation.Name, recipeInformation.ToString());
            }
        }

        private static void LoadToPlayer()
        {
            var player = Game1.player;
            foreach (var recipeInformation in CustomRecipeInformations)
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