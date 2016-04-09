namespace Igorious.StardewValley.DynamicAPI.Interfaces
{
    public interface IMachine : ICraftable, IMachineOutput, IInformation
    {
        string Description { get; set; }
    }
}