using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;

namespace Igorious.StardewValley.NewMachinesMod
{
    public partial class NewMachinesModConfig : DynamicConfiguration
    {
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

            ItemOverrides = new List<ObjectInformation>
            {
                new ObjectInformation
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
                    Category = ObjectCategory.ArtisanGoods,
                    Description = "Drink from honey.",
                    Type = "Basic",
                    Name = "Mead",
                    Price = 500,
                    Edibility = 20,
                    Subcategory = "drink",
                    ResourceIndex = 240,
                },

                new ObjectInformation
                {
                    ID = VodkaID,
                    Category = ObjectCategory.ArtisanGoods,
                    Description = "Light alcohol drink.",
                    Type = "Basic",
                    Name = "Vodka",
                    Price = 400,
                    Edibility = 15,
                    Subcategory = "drink",
                    SkillUps = new SkillUpInformation {Speed = -1, Combat = -1, Luck = +1},
                    Duration = 60,
                    ResourceIndex = 241,
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
        }
    }
}
