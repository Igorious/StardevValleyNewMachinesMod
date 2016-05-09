using Igorious.StardewValley.DynamicAPI.Constants;

namespace Igorious.StardewValley.DynamicAPI.Objects
{
    public abstract class SmartBigCrafrableBase : SmartObject
    {
        protected SmartBigCrafrableBase(int id) : base(id) { }

        protected override TextureType TextureType { get; } = TextureType.Craftables;
        protected override int VerticalShift { get; } = -1;

        protected MachineState State
        {
            get
            {
                if (readyForHarvest) return MachineState.Ready;
                if (heldObject != null) return MachineState.Working;
                return MachineState.Empty;
            }
        }
    }
}
