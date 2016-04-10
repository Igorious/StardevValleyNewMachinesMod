using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;

namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface ICraftable : IItem
    {
        Dictionary<int, int> Materials { get; set; }
        Skill Skill { get; set; }
        int? SkillLevel { get; set; }
    }
}