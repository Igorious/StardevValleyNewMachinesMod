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
        private readonly List<IItemInformation> _itemsInformations = new List<IItemInformation>();
        private readonly List<ITreeInformation> _treesInformations = new List<ITreeInformation>();
        private readonly List<ICropInformation> _cropInformations = new List<ICropInformation>();
        private readonly List<ItemInformation> _overridableInformations = new List<ItemInformation>();

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
        public void Register(IItemInformation information)
        {
            _itemsInformations.Add(information);
        }

        /// <summary>
        /// Register information about item.
        /// </summary>
        public void Register(ITreeInformation information)
        {
            _treesInformations.Add(information);
        }

        /// <summary>
        /// Register information about crop.
        /// </summary>
        public void Register(ICropInformation information)
        {
            _cropInformations.Add(information);
        }

        /// <summary>
        /// Override information about item.
        /// </summary>
        public void Override(ItemInformation information)
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
                for (var i = 0; i < information.ResourceLength; ++i)
                {
                    var key = information.ID + i;
                    var newValue = information.ToString();
                    string oldValue;
                    if (!gameInformation.TryGetValue(key, out oldValue))
                    {
                        gameInformation.Add(key, newValue);
                    }
                    else if (newValue != oldValue)
                    {
                        Log.SyncColour($"Information for ID={key} already has another mapping {key}->{oldValue} (current:{newValue})", ConsoleColor.DarkRed);
                    }
                }
            }
        }

        private static Dictionary<int, string> GetDataCache(string assetPath)
        {
            Game1.content.Load<Dictionary<int, string>>(assetPath);
            var loadedAssets = Game1.content.GetField<Dictionary<string, object>>("loadedAssets");
            return (Dictionary<int, string>)loadedAssets[assetPath];
        }

        private static string OverrideInformation(string actualInfo, ItemInformation overridableInformation)
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