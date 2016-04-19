using System;
using System.Collections.Generic;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Extensions;
using StardewModdingAPI.Events;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class BundlesService
    {
        #region Singleton Access

        private BundlesService()
        {
            GameEvents.LoadContent += OnLoadContent;
        }

        private static BundlesService _instance;

        public static BundlesService Instance => _instance ?? (_instance = new BundlesService());

        #endregion

        #region Private Data

        private readonly List<BundleInformation> _bundleInformations = new List<BundleInformation>();
        private readonly List<OverridedBundleInformation> _addedBundleItems = new List<OverridedBundleInformation>();
        private readonly List<OverridedBundleInformation> _removedBundleItems = new List<OverridedBundleInformation>();

        #endregion

        #region	Public Methods

        public void AddBundleItems(OverridedBundleInformation bundle)
        {
            _addedBundleItems.Add(bundle);
        }

        public void RemoveBundleItems(OverridedBundleInformation bundle)
        {
            _removedBundleItems.Add(bundle);
        }

        #endregion

        #region	Auxiliary Methods

        private void OnLoadContent(object sender, EventArgs e)
        {
            Game1.content.Load<Dictionary<string, string>>(@"Data\Bundles");
            var loadedAssets = Game1.content.GetField<Dictionary<string, object>>("loadedAssets");
            var bundles = (Dictionary<string, string>)loadedAssets[@"Data\Bundles"];

            foreach (var kv in bundles)
            {
                _bundleInformations.Add(BundleInformation.Parse(kv.Value, kv.Key));
            }

            foreach (var addedBundle in _addedBundleItems)
            {
                var bundle = _bundleInformations.First(b => b.Key == addedBundle.Key);
                bundle.Items.AddRange(addedBundle.Items);
                bundles[bundle.Key] = bundle.ToString();
            }

            foreach (var removedBundle in _removedBundleItems)
            {
                var bundle = _bundleInformations.First(b => b.Key == removedBundle.Key);
                var currentItems = bundle.Items.ToList();
                bundle.Items.Clear();

                foreach (var currentItem in currentItems)
                {
                    if (removedBundle.Items.Any(i => i.ID == currentItem.ID)) continue;
                    bundle.Items.Add(currentItem);
                }

                bundles[bundle.Key] = bundle.ToString();
            }
        }

        #endregion
    }
}
