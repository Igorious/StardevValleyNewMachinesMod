using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.NewMachinesMod.Data;

namespace Igorious.StardewValley.NewMachinesMod
{
    public partial class NewMachinesModConfig
    {
        public enum LocalizationString
        {
            TankRequiresWater,
        }

        public List<MachineInformation> SimpleMachines { get; set; } = new List<MachineInformation>();
        public List<OverridedMachineInformation> MachineOverrides { get; set; } = new List<OverridedMachineInformation>();
        public MachineInformation Tank { get; set; }
        public List<ItemInformation> ItemOverrides { get; set; } = new List<ItemInformation>();
        public List<ItemInformation> Items { get; set; } = new List<ItemInformation>();
        public List<CropInformation> Crops { get; set; } = new List<CropInformation>();
        public Dictionary<LocalizationString, string> LocalizationStrings { get; set; } = new Dictionary<LocalizationString, string>();
        public List<GiftPreferences> GiftPreferences { get; set; } = new List<GiftPreferences>();
    }
}
