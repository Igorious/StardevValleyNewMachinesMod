using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;

namespace Igorious.StardewValley.NewMachinesMod.Data
{
    public class OverridedMachineInformation
    {
        public DynamicID<CraftableID> ID { get; set; }

        public MachineOutputInformation Output { get; set; }
    }
}