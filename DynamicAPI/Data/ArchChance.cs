namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class ArchChance
    {
        public string Location { get; set; }
        public decimal Chance { get; set; }

        public override string ToString()
        {
            return $"{Location} {Chance:.#####}";
        }
    }
}