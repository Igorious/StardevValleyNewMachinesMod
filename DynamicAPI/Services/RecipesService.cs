using System;
using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class RecipesService
    {
        #region Singleton Access

        private RecipesService()
        {
            GameEvents.LoadContent += (s, e) => LoadToGame();
            PlayerEvents.LoadedGame += (s, e) => LoadToPlayer();
        }

        private static RecipesService _instance;

        public static RecipesService Instance => _instance ?? (_instance = new RecipesService());

        #endregion

        #region Private Data

        private readonly List<CraftingRecipeInformation> _craftingRecipeInformations = new List<CraftingRecipeInformation>();
        //private readonly List<CookingRecipeInformation> _cookingRecipeInformations = new List<CookingRecipeInformation>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Register a custom crafting recipe.
        /// </summary>
        public void Register(CraftingRecipeInformation craftingRecipeInformation)
        {
            _craftingRecipeInformations.Add(craftingRecipeInformation);
        }

        /// <summary>
        /// Register a custom cooking recipe.
        /// </summary>
        //public void Register(CookingRecipeInformation cookingRecipeInformation)
        //{
        //    _cookingRecipeInformations.Add(cookingRecipeInformation);
        //}

        #endregion

        #region	Auxiliary Methods

        private void LoadToGame()
        {
            var craftingRecipes = CraftingRecipe.craftingRecipes;
            foreach (var recipeInformation in _craftingRecipeInformations)
            {
                var key = recipeInformation.Name;
                var newValue = recipeInformation.ToString();
                string oldValue;
                if (!craftingRecipes.TryGetValue(key, out oldValue))
                {
                    craftingRecipes.Add(key, newValue);
                }
                else if (newValue != oldValue)
                {
                    Log.SyncColour($"Recipe for ID={key} already has another mapping {key}->{oldValue} (current:{newValue})", ConsoleColor.DarkRed);
                }
            }
        }

        private void LoadToPlayer()
        {
            var player = Game1.player;
            var playerSkills = new Dictionary<Skill, int>
            {
                {Skill.Undefined, -1 },
                {Skill.Combat, player.CombatLevel },
                {Skill.Farming, player.FarmingLevel },
                {Skill.Fishing, player.FishingLevel },
                {Skill.Foraging, player.ForagingLevel },
                {Skill.Luck, player.LuckLevel },
                {Skill.Mining, player.MiningLevel },
            };

            foreach (var recipeInformation in _craftingRecipeInformations)
            {
                if (player.craftingRecipes.ContainsKey(recipeInformation.Name)) continue;

                var wayToGet = recipeInformation.WayToGet;
                if (wayToGet.IsDefault || playerSkills[wayToGet.Skill] >= wayToGet.SkillLevel)
                {
                    player.craftingRecipes.Add(recipeInformation.Name, 0);
                }
            }
        }

        #endregion
    }
}