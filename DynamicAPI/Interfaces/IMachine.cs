using Igorious.StardewValley.DynamicAPI.Data;

namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface IMachine : ICraftable, IMachineOutput
    {
        string Description { get; }
        MachineDraw Draw { get; }
    }
}