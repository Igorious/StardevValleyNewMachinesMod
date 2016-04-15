using System.ComponentModel;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Interfaces;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class TreeInformation : IDrawable, ITreeInformation
    {
        public DynamicID<ItemID> SapleID { get; set; }

        [DefaultValue(-1)]
        public int TextureIndex { get; set; } = -1;

        public Season Season { get; set; }

        public DynamicID<ItemID> FruitID { get; set; }

        private int UnknownVar { get; set; } = 1234;

        public int? ResourceIndex { get; set; }

        [DefaultValue(1)]
        public int ResourceLength { get; set; } = 1;

        int IInformation.ID => SapleID;

        public static TreeInformation Parse(string treeInformation)
        {
            var info = new TreeInformation();
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
    }
}
