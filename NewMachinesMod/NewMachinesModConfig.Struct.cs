using System.Collections.Generic;
using System.ComponentModel;
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
            int IDrawable.TextureIndex => ID;
            public int? ResourceIndex { get; set; }
            [DefaultValue(1)]
            public int ResourceLength { get; set; } = 1;
            public string Description { get; set; }
            public string Skill { get; set; }
            public int? SkillLevel { get; set; }
            public int? MinutesUntilReady { get; set; }
            public Dictionary<int, int> Materials { get; set; } = new Dictionary<int, int>();
            public OutputInfo Output { get; set; }
        }

        public class OverrideMachineInfo : IMachineOutput
        {
            public int ID { get; set; }
            public OutputInfo Output { get; set; }
            public int? MinutesUntilReady { get; set; }
        }

        public MachineInfo Mill { get; set; }
        public MachineInfo Tank { get; set; }
        public MachineInfo VinegarJug { get; set; }
        public OverrideMachineInfo KegEx { get; set; }
        public List<ObjectInformation> ItemOverrides { get; set; } = new List<ObjectInformation>();
        public List<ObjectInformation> Items { get; set; } = new List<ObjectInformation>();

    }
}
