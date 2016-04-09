using System;
using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class InformationService
    {
        #region Singleton Access

        private InformationService()
        {
            GameEvents.LoadContent += (s, e) => LoadToGame();
        }

        private static InformationService _instance;

        public static InformationService Instance => _instance ?? (_instance = new InformationService());

        #endregion

        #region Private Data

        private readonly List<CraftableInformation> _craftableInformations = new List<CraftableInformation>();
        private readonly List<ObjectInformation> _itemsInformations = new List<ObjectInformation>();
        private readonly List<TreeInformation> _treesInformations = new List<TreeInformation>();
        private readonly List<CropInformation> _cropInformations = new List<CropInformation>();
        private readonly List<ObjectInformation> _overridableInformations = new List<ObjectInformation>();

        #endregion

        #region	Public Methods

        /// <summary>
        /// Register information about craftable item.
        /// </summary>
        public void Register(CraftableInformation information)
        {
            _craftableInformations.Add(information);
        }

        /// <summary>
        /// Register information about item.
        /// </summary>
        public void Register(ObjectInformation information)
        {
            _itemsInformations.Add(information);
        }

        /// <summary>
        /// Register information about item.
        /// </summary>
        public void Register(TreeInformation information)
        {
            _treesInformations.Add(information);
        }

        /// <summary>
        /// Register information about crop.
        /// </summary>
        public void Register(CropInformation information)
        {
            _cropInformations.Add(information);
        }

        /// <summary>
        /// Override information about item.
        /// </summary>
        public void Override(ObjectInformation information)
        {
            _overridableInformations.Add(information);
        }

        #endregion

        #region	Auxiliary Methods

        private void LoadToGame()
        {
            LoadToGame(Game1.bigCraftablesInformation, _craftableInformations);
            LoadToGame(Game1.objectInformation, _itemsInformations);
            LoadToGame(GetDataCache(@"Data\Crops"), _cropInformations);
            LoadToGame(GetDataCache(@"Data\fruitTrees"), _treesInformations);

            var objectInformation = Game1.objectInformation;
            foreach (var overridableInformation in _overridableInformations)
            {
                var id = overridableInformation.ID;
                string actualInfo;
                if (!objectInformation.TryGetValue(id, out actualInfo)) continue;

                objectInformation.Remove(id);
                objectInformation.Add(id, OverrideInformation(actualInfo, overridableInformation));
            }  
        }

        private static void LoadToGame(IDictionary<int, string> gameInformation, IReadOnlyList<IInformation> customInformation)
        {
            foreach (var information in customInformation)
            {
                var length = (information as IDrawable)?.ResourceLength ?? 1;
                for (var i = 0; i < length; ++i)
                {
                    if (gameInformation.ContainsKey(information.ID + i)) continue;
                    Log.SyncColour($"Loaded info #{information.ID + i}: {information}", ConsoleColor.DarkCyan);
                    gameInformation.Add(information.ID + i, information.ToString());
                }
            }
        }

        private static Dictionary<int, string> GetDataCache(string assetPath)
        {
            Game1.content.Load<Dictionary<int, string>>(assetPath);
            var loadedAssets = Game1.content.GetField<Dictionary<string, object>>("loadedAssets");
            return (Dictionary<int, string>)loadedAssets[assetPath];
        }

        private static string OverrideInformation(string actualInfo, ObjectInformation overridableInformation)
        {
            var parts = actualInfo.Split('/');
            if (overridableInformation.Name != null) parts[0] = overridableInformation.Name;
            if (overridableInformation.Price != 0) parts[1] = overridableInformation.Price.ToString();
            if (overridableInformation.Description != null) parts[4] = overridableInformation.Description;
            return string.Join("/", parts);
        }

        #endregion
    }
}