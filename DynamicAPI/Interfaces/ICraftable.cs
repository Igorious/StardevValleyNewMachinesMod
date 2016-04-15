using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;

namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface ICraftable : IItem
    {
        Dictionary<DynamicID<ItemID>, int> Materials { get; set; }
        Skill Skill { get; set; }
        int? SkillLevel { get; set; }
    }
}