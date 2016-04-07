using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using StardewModdingAPI.Events;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public static class CustomObjectInformations
    {
        #region Private Data

        private static readonly List<CraftableInformation> _craftableInformations = new List<CraftableInformation>();
        private static readonly List<ObjectInformation> _itemsInformations = new List<ObjectInformation>();
        private static readonly List<OverridableInformation> _overridableInformations = new List<OverridableInformation>();
        private static bool IsInitialized { get; set; }

        #endregion

        #region	Public Methods

        /// <summary>
        /// Register information about craftable item.
        /// </summary>
        public static void AddBigCraftable(int id, string name, string description)
        {
            Initialize();
            _craftableInformations.Add(new CraftableInformation { ID = id, Name = name, Description = description });
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
            _itemsInformations.Add(information);
        }

        /// <summary>
        /// Override information about item.
        /// </summary>
        public static void OverrideItemInformation(int id, string newName = null, string newDescription = null, int? newPrice = null)
        {
            Initialize();
            _overridableInformations.Add(new OverridableInformation { ID = id, Description = newDescription, Price = newPrice, Name = newName });
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
            foreach (var craftableInformation in _craftableInformations)
            {
                if (craftablesInformation.ContainsKey(craftableInformation.ID)) continue;
                craftablesInformation.Add(craftableInformation.ID, craftableInformation.ToString());
            }

            var objectInformation = Game1.objectInformation;
            foreach (var overridableInformation in _overridableInformations)
            {
                var id = overridableInformation.ID;
                string actualInfo;
                if (!objectInformation.TryGetValue(id, out actualInfo)) continue;

                objectInformation.Remove(id);
                objectInformation.Add(id, OverrideInformation(actualInfo, overridableInformation));
            }

            foreach (var itemInformation in _itemsInformations)
            {
                if (objectInformation.ContainsKey(itemInformation.ID)) continue;
                objectInformation.Add(itemInformation.ID, itemInformation.ToString());
            }
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