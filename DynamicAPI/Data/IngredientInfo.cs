using Igorious.StardewValley.DynamicAPI.Constants;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class IngredientInfo
    {
        public IngredientInfo(ItemID id, int count) : this((int)id, count) { }

        public IngredientInfo(int id, int count)
        {
            ID = id;
            Count = count;
        }

        public DynamicID<ItemID> ID { get; }

        public int Count { get; }

        public override string ToString() => $"{ID} {Count}";
    }
}