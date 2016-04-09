namespace Igorious.StardewValley.DynamicAPI.Constants
{
    public enum Season
    {
        Undefined = 0,
        Spring,
        Summer,
        Fall,
        Winter,
    }

    public static class SeasonExtensions
    {
        public static string ToFlat(this Season season)
        {
            return season.ToString().ToLower();
        }
    }
}
