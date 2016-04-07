using Igorious.StardewValley.DynamicAPI.Data;

namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface IMachineOutput
    {
        OutputInfo Output { get; }
        int MinutesUntilReady { get; }
    }
}