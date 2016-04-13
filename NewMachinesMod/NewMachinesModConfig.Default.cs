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

        public override void CreateDefaultConfiguration()
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
                },
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
                },
                Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
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
                },
                Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
            };

            Dryer = new MachineInfo
            {
                ID = 169,
                ResourceIndex = 5,
                ResourceLength = 3,
                MinutesUntilReady = 1440,
                Name = "Dryer",
                Description = "It has been created to dry everything.",
                Skill = Skill.Farming,
                SkillLevel = 6,
                Materials = new Dictionary<int, int>
                {
                    { (int)ItemID.Wood, 30 },
                    { (int)ItemID.Hardwood, 2 },
                },
                Output = new OutputInfo
                {
                    Items = new Dictionary<int, OutputItem>
                    {
                        { (int)ItemID.Fiber, new OutputItem {ID = (int)ItemID.Hay} },
                    },
                },
            };

            ItemOverrides = new List<ItemInformation>
            {
                new ItemInformation
                {
                    ID = (int)ItemID.WheatFlour,
                    Name = "Flour",
                    Description = "A common cooking ingredient made from crushed seeds.",
                },
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
                        { (int)ItemID.Honey, new OutputItem {ID = MeadID, MinutesUntilReady = 4000} },
                        { (int)ItemID.Potato, new OutputItem {ID = VodkaID, MinutesUntilReady = 3000} },
                    },
                }
            };

            LocalizationStrings = new Dictionary<LocalizationString, string>
            {
                { LocalizationString.TankRequiresWater, "Fill with water first" },
            };
        }
    }
}
