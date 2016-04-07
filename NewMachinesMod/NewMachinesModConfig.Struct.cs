using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Interfaces;

namespace Igorious.StardewValley.NewMachinesMod
{
    public partial class NewMachinesModConfig
    {
        public class MachineInfo : IMachine
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Skill { get; set; }
            public int? SkillLevel { get; set; }
            public int MinutesUntilReady { get; set; }
            public Dictionary<int, int> Materials { get; set; } = new Dictionary<int, int>();
            public OutputInfo Output { get; set; }
        }

        public class OverrideMachineInfo : IMachineOutput
        {
            public int ID { get; set; }
            public OutputInfo Output { get; set; }
            public int MinutesUntilReady { get; set; }
        }

        public MachineInfo Mill { get; set; }
        public MachineInfo Tank { get; set; }
        public MachineInfo VinegarJug { get; set; }
        public OverrideMachineInfo KegEx { get; set; }
        public List<OverridableInformation> Overrides { get; set; } = new List<OverridableInformation>();
        public List<ObjectInformation> Items { get; set; } = new List<ObjectInformation>();
    }
}
