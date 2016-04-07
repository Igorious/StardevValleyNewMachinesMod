using System.Collections.Generic;

namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface ICraftable : IItem
    {
        Dictionary<int, int> Materials { get; set; }
        string Skill { get; set; }
        int? SkillLevel { get; set; }
    }
}