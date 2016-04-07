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

            Overrides = new List<OverridableInformation>
            {
                new OverridableInformation
                {
                    ID = (int)ItemID.WheatFlour,
                    Name = "Flour",
                    Description = "A common cooking ingredient made from crushed seeds.",
                }
            };

            Items = new List<ObjectInformation>
            {
                new ObjectInformation
                {
                    ID = 816,
                    Category = Object.artisanGoodsCategory,
                    Description = "Drink from honey.",
                    Type = "Basic",
                    Name = "Mead",
                    Price = 500,
                    Edibility = 20,
                    Subcategory = "drink",
                    SkillUps = ObjectInformation.NoSkillUps,
                    Duration = 0,
                },

                new ObjectInformation
                {
                    ID = 817,
                    Category = Object.artisanGoodsCategory,
                    Description = "Light alcohol drink.",
                    Type = "Basic",
                    Name = "Vodka",
                    Price = 400,
                    Edibility = 15,
                    Subcategory = "drink",
                    SkillUps = new int[10] {-1,-1, -1, -1, -1, -1, -1, -1, -1, -1},
                    Duration = 60,
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
                        { (int)ItemID.Honey, new OutputItem {ID = 816} },
                        { (int)ItemID.Potato, new OutputItem {ID = 817} },
                    },
                }
            };

            return this;
        }
    }
}
