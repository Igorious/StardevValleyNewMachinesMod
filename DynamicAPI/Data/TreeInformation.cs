using System.ComponentModel;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Interfaces;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class TreeInformation : IDrawable, IInformation
    {
        public int SapleID { get; set; }

        [DefaultValue(-1)]
        public int TextureIndex { get; set; } = -1;

        int IDrawable.ResourceLength { get; } = 1;

        public Season Season { get; set; }

        public int FruitID { get; set; }

        public int? ResourceIndex { get; set; }

        int IInformation.ID => SapleID;

        public static TreeInformation Parse(string treeInformation)
        {
            var info = new TreeInformation();
            var parts = treeInformation.Split('/');
            info.TextureIndex = int.Parse(parts[0]);
            info.Season = parts[1].ToEnum<Season>();
            info.FruitID = int.Parse(parts[2]);
            return info;
        }

        public override string ToString()
        {
            return $"{TextureIndex}/{Season.ToFlat()}/{FruitID}/1234";
        }
    }
}
