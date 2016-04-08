using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using StardewValley;

namespace Igorious.StardewValley.NewMachinesMod
{
    public partial class NewMachinesModConfig
    {
        public NewMachinesModConfig FillWithDefaults()
        {
            Mill = new MachineInfo
            {
                ID = 164,
                ResourceIndex = 0,
                ResourceLength = 2,
                MinutesUntilReady = 360,
                Name = "Mill",
                Description = "Small machine, that crushes seeds to flour.",
                Skill = Skill.Farming,
                SkillLevel = 7,
                Materials = new Dictionary<int, int>
                {
                    { (int)ItemID.Wood, 50 },
                    { (int)ItemID.Hardwood, 5 },
                    { (int)ItemID.IronBar, 1 },
                },
                Output = new OutputInfo
                {
                    ID = (int)ItemID.WheatFlour,
                    Items = new Dictionary<int, OutputItem>
                    {
                        { (int)ItemID.Wheat, new OutputItem {Name = "Wheat Flour"} },
                        { (int)ItemID.Amaranth, new OutputItem {Name = "Amaranth Flour"} },
                        { (int)ItemID.Corn, new OutputItem {Name = "Corn Flour"} },
                    },
                    Price = "p + 25",
                    Quality = "0",
                    Count = "(r1 <= 0.1 * q)? 2 : 1",
                }
            };

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
                Materials = new Dictionary<int, int>
                {
                    { (int)ItemID.CopperBar, 1 },
                    { (int)ItemID.IronBar, 1 },
                    { (int)ItemID.Coal, 5 },
                },
                Output = new OutputInfo
                {
                    ID = (int)ItemID.Sugar,
                    Items = new Dictionary<int, OutputItem>
                    {
                        { (int)ItemID.Beet, new OutputItem {Name = "Beet Sugar"} },
                    },
                    Price = "p + 25",
                    Quality = "0",
                    Count = "(r1 <= 0.1 * q)? 2 : 1",
                }
            };

            VinegarJug = new MachineInfo
            {
                ID = 168,
                ResourceIndex = 4,
                MinutesUntilReady = 600,
                Name = "Vinegar Jug",
                Description = "Ceramic thing, that used to vinegar creation.",
                Skill = Skill.Farming,
                SkillLevel = 9,
                Materials = new Dictionary<int, int>
                {
                    { (int)ItemID.Clay, 50 },
                    { (int)ItemID.Slime, 10 },
                    { (int)ItemID.CopperBar, 1 },
                },
                Output = new OutputInfo
                {
                    ID = (int)ItemID.Vinegar,
                    Items = new Dictionary<int, OutputItem>
                    {
                        { (int)ItemID.Apple, new OutputItem {Name = "Apple Cider Vinegar"} },
                        { (int)ItemID.Wine, new OutputItem {Name = "Wine Vinegar", Price = 500} },
                    },
                    Quality = "(p > 1000)? 2 : (p > 500)? 1 : 0",
                    Count = "(r1 <= 0.1 * q)? 2 : 1",
                }
            };

            ItemOverrides = new List<OverridableInformation>
            {
                new OverridableInformation
                {
                    ID = (int)ItemID.WheatFlour,
                    Name = "Flour",
                    Description = "A common cooking ingredient made from crushed seeds.",
                }
            };

            const int MeadID = 900;
            const int VodkaID = 901;

            Items = new List<ObjectInformation>
            {
                new ObjectInformation
                {
                    ID = MeadID,
                    Category = Object.artisanGoodsCategory,
                    Description = "Drink from honey.",
                    Type = "Basic",
                    Name = "Mead",
                    Price = 500,
                    Edibility = 20,
                    Subcategory = "drink",
                    SkillUps = ObjectInformation.NoSkillUps,
                    Duration = 0,
                    ResourceIndex = 0,
                },

                new ObjectInformation
                {
                    ID = VodkaID,
                    Category = Object.artisanGoodsCategory,
                    Description = "Light alcohol drink.",
                    Type = "Basic",
                    Name = "Vodka",
                    Price = 400,
                    Edibility = 15,
                    Subcategory = "drink",
                    SkillUps = new int[10] {-1,-1, -1, -1, -1, -1, -1, -1, -1, -1},
                    Duration = 60,
                    ResourceIndex = 1,
                }
            };

            KegEx = new OverrideMachineInfo
            {
                ID = (int)CraftableID.Keg,
                MinutesUntilReady = 1440,
                Output = new OutputInfo
                {
                    Items = new Dictionary<int, OutputItem>
                    {
                        { (int)ItemID.Honey, new OutputItem {ID = MeadID} },
                        { (int)ItemID.Potato, new OutputItem {ID = VodkaID} },
                    },
                }
            };

            Items.Add(new ObjectInformation
            {
                ID = 1001,
                Category = Object.SeedsCategory,
                Description = "Seed no1001",
                Name = "Seed no1001",
                Price = 1001,
                Type = "Seeds", 
                ResourceIndex = 4,
            });
            Items.Add(new ObjectInformation
            {
                ID = 1002,
                Category = Object.SeedsCategory,
                Description = "Takes 28 days to produce a mature cherry tree. Bears fruit in the spring. Only grows if the 8 surrounding \"tiles\" are empty.",
                Name = "Test Sample",
                Price = 1002,
                Type = "Basic",  
                ResourceIndex = 3,
            });
            Items.Add(new ObjectInformation
            {
                ID = 1003,
                Category = Object.VegetableCategory,
                Description = "Crop no1003",
                Name = "Crop no1003",
                Price = 1003,
                Type = "Basic",   
                Edibility = 30,
                ResourceIndex = 7,
            });

            Crops = new List<CropInformation>
            {
                new CropInformation
                {
                    SeedID = 1001,
                    TextureIndex = 70,
                    ResourceIndex = 1,
                    CropID = 1003,
                    Seasons = new List<string> {"spring", "summer", "fall" },
                    Phases = new List<int> {1, 1, 1 },
                }
            };

            Trees = new List<TreeInformation>
            {
                new TreeInformation
                {
                    SapleID = 1002,
                    FruitID = 1003,
                    TextureIndex = 0,
                    Season = "fall",               
                }
            };

            return this;
        }
    }
}
