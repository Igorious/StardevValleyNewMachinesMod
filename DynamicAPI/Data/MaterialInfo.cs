using Igorious.StardewValley.DynamicAPI.Constants;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class MaterialInfo
    {
        public MaterialInfo(ItemID id, int count) : this((int)id, count) { }

        public MaterialInfo(int id, int count)
        {
            ID = id;
            Count = count;
        }

        public int ID { get; }
        public int Count { get; }

        public override string ToString() => $"{ID} {Count}";
    }
}