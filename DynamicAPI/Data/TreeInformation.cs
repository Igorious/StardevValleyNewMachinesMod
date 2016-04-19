﻿using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Newtonsoft.Json;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class TreeInformation : IDrawable, ITreeInformation
    {
        #region Properties

        [JsonProperty(Required = Required.Always)]
        public DynamicID<ItemID> SapleID { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int TextureIndex { get; set; }

        [JsonProperty(Required = Required.Always)]
        public Season Season { get; set; }

        [JsonProperty(Required = Required.Always)]
        public DynamicID<ItemID> FruitID { get; set; }

        [JsonIgnore]
        private int UnknownVar { get; set; } = 1234;

        [JsonProperty]
        public int? ResourceIndex { get; set; }

        #endregion

        #region Serialization

        public static TreeInformation Parse(string treeInformation, int sapleID = 0)
        {
            var info = new TreeInformation {SapleID = sapleID};
            var parts = treeInformation.Split('/');
            info.TextureIndex = int.Parse(parts[0]);
            info.Season = parts[1].ToEnum<Season>();
            info.FruitID = int.Parse(parts[2]);
            info.UnknownVar = int.Parse(parts[3]);
            return info;
        }

        public override string ToString()
        {
            return $"{TextureIndex}/{Season.ToLower()}/{FruitID}/{UnknownVar}";
        }

        #endregion

        #region Explicit Interface Implemetation

        int IDrawable.ResourceLength { get; } = 1;

        int IInformation.ID => SapleID;

        #endregion
    }
}
