using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;

namespace Igorious.StardewValley.NewMachinesMod
{
    public partial class NewMachinesModConfig : DynamicConfiguration
    {
        private const int MeadID = 900;
        private const int VodkaID = 901;
        private const int CactusJuiseID = 902;
        private const int CactusSeedID = 904;

        public override void CreateDefaultConfiguration()
        {
            SimpleMachines.Add(new MachineInfo
            {
                ID = 164,
                ResourceIndex = 0,
                ResourceLength = 2,
                MinutesUntilReady = 360,
                Name = "Mill",
                Description = "Small machine, that crushes seeds to flour.",
                Skill = Skill.Farming,
                SkillLevel = 7,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    { ItemID.Wood, 50 },
                    { ItemID.Hardwood, 5 },
                    { ItemID.IronBar, 1 },
                },
                Output = new OutputInfo
                {
                    ID = ItemID.WheatFlour,
                    Items = new Dictionary<DynamicID<ItemID>, OutputItem>
                    {
                        { ItemID.Wheat, new OutputItem() },
                        { ItemID.Amaranth, new OutputItem() },
                        { ItemID.Corn, new OutputItem() },
                    },
                    NameFormat = "{0} Flour",
                    Price = "p + 25",
                    Quality = "0",
                    Count = "(r1 <= 0.1 * q)? 2 : 1",
                },
                Draw = new MachineDraw
                {
                    Ready = +1,
                }
            });

            Tank = new MachineInfo
            {
                ID = 166,
                ResourceIndex = 2,
                ResourceLength = 2,
                MinutesUntilReady = 360,
                Name = "Tank",
                Description = "Fill with water and use for sugar extraction from beet.",
                Skill = Skill.Farming,
                SkillLevel = 8,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    { ItemID.CopperBar, 1 },
                    { ItemID.IronBar, 1 },
                    { ItemID.Coal, 5 },
                },
                Output = new OutputInfo
                {
                    ID = ItemID.Sugar,
                    Items = new Dictionary<DynamicID<ItemID>, OutputItem>
                    {
                        { ItemID.Beet, new OutputItem() },
                    },
                    NameFormat = "{0} Sugar",
                    Price = "p + 25",
                    Quality = "0",
                    Count = "(r1 <= 0.1 * q)? 2 : 1",
                },
                Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
            };

            SimpleMachines.Add(new MachineInfo
            {
                ID = 168,
                ResourceIndex = 4,
                MinutesUntilReady = 600,
                Name = "Vinegar Jug",
                Description = "Ceramic thing, that used to vinegar creation.",
                Skill = Skill.Farming,
                SkillLevel = 9,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    { ItemID.Clay, 50 },
                    { ItemID.Slime, 10 },
                    { ItemID.CopperBar, 1 },
                },
                Output = new OutputInfo
                {
                    ID = ItemID.Vinegar,
                    Items = new Dictionary<DynamicID<ItemID>, OutputItem>
                    {
                        { ItemID.Apple, new OutputItem {Name = "Apple Cider Vinegar"} },
                        { ItemID.Wine, new OutputItem {Name = "Wine Vinegar", Price = 500} },
                    },
                    Quality = "(p > 1000)? 2 : (p > 500)? 1 : 0",
                    Count = "(r1 <= 0.1 * q)? 2 : 1",
                },
                Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
            });

            SimpleMachines.Add(new MachineInfo
            {
                ID = 169,
                ResourceIndex = 5,
                ResourceLength = 3,
                MinutesUntilReady = 1440,
                Name = "Dryer",
                Description = "It has been created to dry everything.",
                Skill = Skill.Farming,
                SkillLevel = 6,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    { ItemID.Wood, 30 },
                    { ItemID.Hardwood, 2 },
                },
                Output = new OutputInfo
                {
                    Items = new Dictionary<DynamicID<ItemID>, OutputItem>
                    {
                        { ItemID.Fiber, new OutputItem {ID = ItemID.Hay} },
                    },
                },
                Draw = new MachineDraw
                {
                    Processing = +1,
                    Ready = +2,
                }
            });

            ItemOverrides = new List<ItemInformation>
            {
                new ItemInformation
                {
                    ID = ItemID.WheatFlour,
                    Name = "Flour",
                    Description = "A common cooking ingredient made from crushed seeds.",
                }
            };

            Items = new List<ItemInformation>
            {
                new ItemInformation
                {
                    ID = MeadID,
                    Category = ObjectCategory.ArtisanGoods,
                    Description = "Drink from honey.",
                    Type = ObjectType.Basic,
                    Name = "Mead",
                    Price = 500,
                    Edibility = 20,
                    MealCategory = MealCategory.Drink,
                    SkillUps = new SkillUpInformation { MaxEnergy = +50 },
                    ResourceIndex = 0,
                },

                new ItemInformation
                {
                    ID = VodkaID,
                    Category = ObjectCategory.ArtisanGoods,
                    Description = "Light alcohol drink.",
                    Type = ObjectType.Basic,
                    Name = "Vodka",
                    Price = 400,
                    Edibility = 15,
                    MealCategory = MealCategory.Drink,
                    SkillUps = new SkillUpInformation { Speed = -1, Defence = +1 },
                    Duration = 60,
                    ResourceIndex = 1,
                },

                new ItemInformation
                {
                    ID = CactusJuiseID,
                    Category = ObjectCategory.ArtisanGoods,
                    Description = "Drink with pleasant taste.",
                    Type = ObjectType.Basic,
                    Name = "Castus Juise",
                    Price = 500,
                    Edibility = 20,
                    MealCategory = MealCategory.Drink,
                    SkillUps = new SkillUpInformation { Farming = +1 },
                    Duration = 60,
                    ResourceIndex = 2,
                    ResourceLength = 2,
                },

                new ItemInformation
                {
                    ID = CactusSeedID,
                    Category = ObjectCategory.Seeds,
                    Description = "Requres special environment. Takes 15 days to mature.",
                    Type = ObjectType.Seeds,
                    Name = "Castus Seed",
                    Price = 30,
                    ResourceIndex = 4,
                },
            };

            MachineOverrides.Add(new OverrideMachineInfo
            {
                ID = CraftableID.Keg,
                MinutesUntilReady = 1440,
                Output = new OutputInfo
                {
                    Items = new Dictionary<DynamicID<ItemID>, OutputItem>
                    {
                        { ItemID.Honey, new OutputItem {ID = MeadID, MinutesUntilReady = 4000} },
                        { ItemID.Potato, new OutputItem {ID = VodkaID, MinutesUntilReady = 3000} },
                        { ItemID.CactusFruit, new OutputItem {ID = CactusJuiseID, MinutesUntilReady = 4500, Color = "31A500" } },
                    },
                },
                Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
            });

            LocalizationStrings = new Dictionary<LocalizationString, string>
            {
                { LocalizationString.TankRequiresWater, "Fill with water first" },
            };

            Crops.Add(new CropInformation
            {
                CropID = ItemID.CactusFruit,
                HarvestMethod = 1,
                MaxHarvest = 3,
                MinHarvest = 3,
                MaxHarvestIncreaseForLevel = 3,
                SeedID = CactusSeedID,
                Phases = new List<int> { 2, 3, 4, 3, 3 },
                TextureIndex = 90,
                ResourceIndex = 0,
                IsRaisedSeeds = true,
            });
        }
    }
}
