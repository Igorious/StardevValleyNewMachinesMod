using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;

namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface IMachineOutput
    {
        OutputInfo Output { get; }
        int? MinutesUntilReady { get; }
        List<Sound> Sounds { get; }
    }
}