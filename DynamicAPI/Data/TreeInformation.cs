using Igorious.StardewValley.DynamicAPI.Interfaces;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class TreeInformation : IDrawable
    {
        public int SapleID { get; set; }
        public int TextureIndex { get; set; }
        public int? ResourceIndex { get; set; }
        int IDrawable.ResourceLength { get; } = 1;
        public string Season { get; set; }
        public int FruitID { get; set; }

        public override string ToString()
        {
            return $"{TextureIndex}/{Season}/{FruitID}/1234";
        }
    }
}
