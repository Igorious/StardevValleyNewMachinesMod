using System;
using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public static class CustomObjectInformations
    {
        #region Private Data

        private static readonly List<CraftableInformation> CraftableInformations = new List<CraftableInformation>();
        private static readonly List<ObjectInformation> ItemsInformations = new List<ObjectInformation>();
        private static readonly List<TreeInformation> TreesInformations = new List<TreeInformation>();
        private static readonly List<CropInformation> CropInformations = new List<CropInformation>();
        private static readonly List<OverridableInformation> OverridableInformations = new List<OverridableInformation>();
        private static bool IsInitialized { get; set; }

        #endregion

        #region	Public Methods

        /// <summary>
        /// Register information about craftable item.
        /// </summary>
        public static void AddBigCraftable(int id, string name, string description)
        {
            Initialize();
            CraftableInformations.Add(new CraftableInformation { ID = id, Name = name, Description = description });
        }

        /// <summary>
        /// Register information about craftable item.
        /// </summary>
        public static void AddBigCraftable(IInformation itemInformation)
        {
            AddBigCraftable(itemInformation.ID, itemInformation.Name, itemInformation.Description);
        }

        /// <summary>
        /// Register information about item.
        /// </summary>
        public static void AddItem(ObjectInformation information)
        {
            Initialize();
            ItemsInformations.Add(information);
        }

        /// <summary>
        /// Register information about item.
        /// </summary>
        public static void AddTree(TreeInformation information)
        {
            Initialize();
            TreesInformations.Add(information);
        }

        /// <summary>
        /// Register information about crop.
        /// </summary>
        public static void AddCrop(CropInformation information)
        {
            Initialize();
            CropInformations.Add(information);
        }

        /// <summary>
        /// Override information about item.
        /// </summary>
        public static void OverrideItemInformation(int id, string newName = null, string newDescription = null, int? newPrice = null)
        {
            Initialize();
            OverridableInformations.Add(new OverridableInformation { ID = id, Description = newDescription, Price = newPrice, Name = newName });
        }

        /// <summary>
        /// Override information about item.
        /// </summary>
        public static void OverrideItemInformation(ItemID id, string newName = null, string newDescription = null, int? newPrice = null)
        {
            OverrideItemInformation((int)id, newName, newDescription, newPrice);
        }

        #endregion

        #region	Auxiliary Methods

        private static void Initialize()
        {
            if (IsInitialized) return;
            GameEvents.LoadContent += (s, e) => LoadToGame();
            IsInitialized = true;
        }

        private static void LoadToGame()
        {
            var craftablesInformation = Game1.bigCraftablesInformation;
            foreach (var craftableInformation in CraftableInformations)
            {
                if (craftablesInformation.ContainsKey(craftableInformation.ID)) continue;
                craftablesInformation.Add(craftableInformation.ID, craftableInformation.ToString());
            }

            var objectInformation = Game1.objectInformation;
            foreach (var overridableInformation in OverridableInformations)
            {
                var id = overridableInformation.ID;
                string actualInfo;
                if (!objectInformation.TryGetValue(id, out actualInfo)) continue;

                objectInformation.Remove(id);
                objectInformation.Add(id, OverrideInformation(actualInfo, overridableInformation));
            }

            foreach (var itemInformation in ItemsInformations)
            {
                if (objectInformation.ContainsKey(itemInformation.ID)) continue;
                Log.SyncColour($"{itemInformation}", ConsoleColor.Green);
                objectInformation.Add(itemInformation.ID, itemInformation.ToString());
            }

            var cropsInformation = GetDataCache(@"Data\Crops");
            foreach (var cropInformation in CropInformations)
            {
                if (cropsInformation.ContainsKey(cropInformation.SeedID)) continue;
                Log.SyncColour($"{cropInformation}", ConsoleColor.Cyan);
                cropsInformation.Add(cropInformation.SeedID, cropInformation.ToString());
            }

            var treesInformation = GetDataCache(@"Data\fruitTrees");
            foreach (var treeInformation in TreesInformations)
            {
                if (treesInformation.ContainsKey(treeInformation.SapleID)) continue;
                Log.SyncColour($"{treeInformation}", ConsoleColor.DarkMagenta);
                treesInformation.Add(treeInformation.SapleID, treeInformation.ToString());
            }
        }

        private static Dictionary<int, string> GetDataCache(string assetPath)
        {
            Game1.content.Load<Dictionary<int, string>>(assetPath);
            var loadedAssets = Game1.content.GetField<Dictionary<string, object>>("loadedAssets");
            return (Dictionary<int, string>)loadedAssets[assetPath];
        }

        private static string OverrideInformation(string actualInfo, OverridableInformation overridableInformation)
        {
            var parts = actualInfo.Split('/');
            if (overridableInformation.Name != null) parts[0] = overridableInformation.Name;
            if (overridableInformation.Price != null) parts[1] = overridableInformation.Price.ToString();
            if (overridableInformation.Description != null) parts[4] = overridableInformation.Description;
            return string.Join("/", parts);
        }

        #endregion
    }
}