using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Interfaces;

namespace Igorious.StardewValley.NewMachinesMod
{
    public partial class NewMachinesModConfig
    {
        public enum LocalizationString
        {
            TankRequiresWater,
        }

        public class MachineInfo : IMachine
        {
            public int ID { get; set; }
            public string Name { get; set; }
            int IDrawable.TextureIndex => ID;
            public int? ResourceIndex { get; set; }
            [DefaultValue(1)]
            public int ResourceLength { get; set; } = 1;
            public string Description { get; set; }
            public Skill Skill { get; set; }
            public int? SkillLevel { get; set; }
            public int? MinutesUntilReady { get; set; }
            public Dictionary<int, int> Materials { get; set; } = new Dictionary<int, int>();
            public OutputInfo Output { get; set; }
            public MachineDraw Draw { get; set; }
            public List<Sound> Sounds { get; set; }

            public CraftableInformation GetCraftableInformation()
            {
                return new CraftableInformation
                {
                    ID = ID,
                    Name = Name,
                    Description = Description,    
                    ResourceLength = ResourceLength,
                };
            }

            public CraftingRecipeInformation GetCraftingRecipe()
            {
                return new CraftingRecipeInformation
                {
                    ID = ID,
                    Name = Name,
                    IsBig = true,
                    Materials = Materials.Select(m => new IngredientInfo(m.Key, m.Value)).ToList(),
                    WayToGet = new WayToGetCraftingRecipe
                    {
                        Skill = Skill,
                        SkillLevel = SkillLevel,
                        IsDefault = (SkillLevel == null),
                    }
                };
            }
        }

        public class OverrideMachineInfo : IMachineOutput
        {
            public int ID { get; set; }
            public OutputInfo Output { get; set; }
            public int? MinutesUntilReady { get; set; }
            public List<Sound> Sounds { get; set; }
        }

        public List<MachineInfo> SimpleMachines { get; set; } = new List<MachineInfo>();
        public List<OverrideMachineInfo> MachineOverrides { get; set; } = new List<OverrideMachineInfo>();
        public MachineInfo Tank { get; set; }
        public List<ItemInformation> ItemOverrides { get; set; } = new List<ItemInformation>();
        public List<ItemInformation> Items { get; set; } = new List<ItemInformation>();
        public List<CropInformation> Crops { get; set; } = new List<CropInformation>();
        public Dictionary<LocalizationString, string> LocalizationStrings { get; set; } = new Dictionary<LocalizationString, string>();
    }
}
