using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.NewMachinesMod.Data;
using PreferencesList = System.Collections.Generic.List<Igorious.StardewValley.DynamicAPI.Data.Supporting.DynamicID<Igorious.StardewValley.DynamicAPI.Constants.ItemID, Igorious.StardewValley.DynamicAPI.Constants.CategoryID>>;

namespace Igorious.StardewValley.NewMachinesMod
{
    public partial class NewMachinesModConfig : DynamicConfiguration
    {
        #region Constants

        private const int MillID = 164;
        private const int TankID = 166;
        private const int VinegarJugID = 168;
        private const int DryerID = 169;
        private const int MixerID = 172;

        private const int MeadID = 900;
        private const int VodkaID = 901;
        private const int ColorWineID = 902;
        private const int CactusSeedID = 904;
        private const int ColorJellyID = 905;
        private const int ColorPicklesID = 907;
        private const int ColorJuiceID = 909;
        private const int ExperementalLiquidID = 911;

        #endregion

        #region Initialize

        public override void CreateDefaultConfiguration()
        {
            SimpleMachines = new List<MachineInformation>
            {
                GetMill(),
                GetVinegarJug(),
                GetDryer(),
            };

            Tank = GetTank();
            Mixer = GetMixer();

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
                GetMead(),
                GetVodka(),
                GetColorJuice(),
                GetColorWine(),
                GetCactusSeed(),
                GetColorJelly(),
                GetColorPickles(),
                GetExperementalLiquid(),
            };

            MachineOverrides = new List<OverridedMachineInformation>
            {
                GetKegOverride(),
                GetPreserveJarOverride()
            };

            LocalizationStrings = new Dictionary<LocalizationString, string>
            {
                {LocalizationString.TankRequiresWater, "Fill with water first"},
            };

            Crops.Add(GetCactusCrop());
            GiftPreferences.AddRange(GetGiftPreferences());
            Bundles = GetBundleInformation();
        }

        #endregion

        #region	Auxiliary Methods

        private static BundlesInformation GetBundleInformation()
        {
            return new BundlesInformation(
                new List<OverridedBundleInformation>
                {
                    new OverridedBundleInformation("Bulletin Board/33",
                        new BundleItemInformation(ColorWineID, quality: 2)),
                    new OverridedBundleInformation("Pantry/5",
                        new BundleItemInformation(ColorJellyID, quality: 2)),
                },
                new List<OverridedBundleInformation>
                {
                    new OverridedBundleInformation("Bulletin Board/33",
                        new BundleItemInformation(ItemID.Wine, quality: 2)),
                    new OverridedBundleInformation("Pantry/5",
                        new BundleItemInformation(ItemID.Jelly, quality: 2)),
                });
        }

        private static CropInformation GetCactusCrop()
        {
            return new CropInformation
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
            };
        }

        private static OverridedMachineInformation GetPreserveJarOverride()
        {
            return new OverridedMachineInformation(CraftableID.PreservesJar,
                new MachineOutputInformation(new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                {
                    {CategoryID.Vegetable, new OutputItem {ID = ColorPicklesID, Color = "@", Name = "{1} {0}"}},
                    {CategoryID.Fruits, new OutputItem {ID = ColorJellyID, Color = "@", Name = "{1} {0}"}},
                })
                {
                    Quality = "q",
                    Price = "40 + 8 * p / 5",
                    MinutesUntilReady = 4000,
                    Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
                });
        }

        private static OverridedMachineInformation GetKegOverride()
        {
            return new OverridedMachineInformation(CraftableID.Keg,
                new MachineOutputInformation(new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                {
                    {ItemID.Hops, new OutputItem {ID = ItemID.PaleAle, MinutesUntilReady = 2360}},
                    {ItemID.Wheat, new OutputItem {ID = ItemID.Beer, MinutesUntilReady = 2250}},
                    {ItemID.Honey, new OutputItem {ID = MeadID, MinutesUntilReady = 4000, Quality = "p / 300"}},
                    {ItemID.Potato, new OutputItem {ID = VodkaID, MinutesUntilReady = 3000}},
                    {CategoryID.Vegetable, new OutputItem {ID = ColorJuiceID, MinutesUntilReady = 6000, Color = "@", Price = "9 * p / 5", Name = "{1} {0}"}},
                    {CategoryID.Fruits, new OutputItem {ID = ColorWineID, MinutesUntilReady = 10000, Color = "@", Price = "12 * p / 5", Name = "{1} {0}"}},
                })
                {
                    Quality = "q",
                    MinutesUntilReady = 1440,
                    Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
                });
        }

        private static ItemInformation GetColorPickles()
        {
            return new ItemInformation
            {
                ID = ColorPicklesID,
                Category = CategoryID.ArtisanGoods,
                Description = "A jar of your home-made pickles.",
                Type = ObjectType.Basic,
                Name = "Pickles",
                Price = 500,
                ResourceIndex = 7,
                ResourceLength = 2,
                IsColored = true,
            };
        }

        private static ItemInformation GetColorJelly()
        {
            return new ItemInformation
            {
                ID = ColorJellyID,
                Category = CategoryID.ArtisanGoods,
                Description = "Gooey.",
                Type = ObjectType.Basic,
                Name = "Jelly",
                Price = 500,
                ResourceIndex = 5,
                ResourceLength = 2,
                IsColored = true,
            };
        }

        private static ItemInformation GetCactusSeed()
        {
            return new ItemInformation
            {
                ID = CactusSeedID,
                Category = CategoryID.Seeds,
                Description = "Requres special environment. Takes 15 days to mature.",
                Type = ObjectType.Seeds,
                Name = "Cactus Seed",
                Price = 30,
                ResourceIndex = 4,
            };
        }

        private static ItemInformation GetColorWine()
        {
            return new ItemInformation
            {
                ID = ColorWineID,
                Category = CategoryID.ArtisanGoods,
                Description = "Drink in moderation.",
                Type = ObjectType.Basic,
                Name = "Wine",
                ResourceIndex = 2,
                ResourceLength = 2,
                IsColored = true,
            };
        }

        private static ItemInformation GetColorJuice()
        {
            return new ItemInformation
            {
                ID = ColorJuiceID,
                Category = CategoryID.ArtisanGoods,
                Description = "A sweet, nutritious beverage.",
                Type = ObjectType.Basic,
                Name = "Juice",
                ResourceIndex = 9,
                ResourceLength = 2,
                IsColored = true,
            };
        }

        private static ItemInformation GetExperementalLiquid()
        {
            return new ItemInformation
            {
                ID = ExperementalLiquidID,
                Category = CategoryID.ArtisanGoods,
                Description = "A strange liquid.",
                Type = ObjectType.Basic,
                Name = "Experemental Liquid",
                ResourceIndex = 11,
                ResourceLength = 2,
                IsColored = true,
            };
        }

        private static ItemInformation GetVodka()
        {
            return new ItemInformation
            {
                ID = VodkaID,
                Category = CategoryID.ArtisanGoods,
                Description = "Light alcohol drink.",
                Type = ObjectType.Basic,
                Name = "Vodka",
                Price = 400,
                Edibility = 15,
                MealCategory = MealCategory.Drink,
                SkillUps = new SkillUpInformation { Speed = -1, Defence = +1 },
                Duration = 60,
                ResourceIndex = 1,
            };
        }

        private static ItemInformation GetMead()
        {
            return new ItemInformation
            {
                ID = MeadID,
                Category = CategoryID.ArtisanGoods,
                Description = "Drink from honey.",
                Type = ObjectType.Basic,
                Name = "Mead",
                Price = 500,
                Edibility = 20,
                MealCategory = MealCategory.Drink,
                SkillUps = new SkillUpInformation { MaxEnergy = +50 },
                ResourceIndex = 0,
            };
        }

        private static MachineInformation GetDryer()
        {
            return new MachineInformation
            {
                ID = DryerID,
                ResourceIndex = 5,
                ResourceLength = 3,
                Name = "Dryer",
                Description = "It has been created to dry everything.",
                Skill = Skill.Farming,
                SkillLevel = 6,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    { ItemID.Wood, 30 },
                    { ItemID.Hardwood, 2 },
                },
                Output = new MachineOutputInformation
                {
                    Items = new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                    {
                        { ItemID.Fiber, new OutputItem {ID = ItemID.Hay} },
                    },
                    MinutesUntilReady = 720,
                },
                Draw = new MachineDraw
                {
                    Working = +1,
                    Ready = +2,
                }
            };
        }

        private static MachineInformation GetMixer()
        {
            return new MachineInformation
            {
                ID = MixerID,
                ResourceIndex = 8,
                ResourceLength = 3,
                Name = "Mixer",
                Description = "Science machine that allows to mix various items.",
                Skill = Skill.Farming,
                SkillLevel = 10,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    { ItemID.CopperBar, 1 },
                    { ItemID.IronBar, 1 },
                    { ItemID.GoldOre, 1 },
                    { ItemID.IridiumOre, 1 },
                    { ItemID.RefinedQuartz, 1 },
                },
                Output = new MachineOutputInformation
                {
                    ID = ExperementalLiquidID,
                    Items = new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                    {
                        { ColorWineID, null },
                        { ColorJuiceID, null },
                    },
                    MinutesUntilReady = 2880,
                },
            };
        }

        private static MachineInformation GetVinegarJug()
        {
            return new MachineInformation
            {
                ID = VinegarJugID,
                ResourceIndex = 4,
                Name = "Vinegar Jug",
                Description = "Ceramic thing, that used to vinegar creation.",
                Skill = Skill.Farming,
                SkillLevel = 9,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    { ItemID.Clay, 30 },
                    { ItemID.Slime, 10 },
                    { ItemID.CopperBar, 1 },
                },
                Output = new MachineOutputInformation
                {
                    ID = ItemID.Vinegar,
                    Items = new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                    {
                        { ItemID.Apple, new OutputItem {Name = "Apple Cider {0}"} },
                        { ItemID.Wine, new OutputItem {Name = "Wine {0}", Price = "500", Quality = "q + 1"} },
                        { ColorWineID, new OutputItem {Name = "Wine {0}", Price = "500", Quality = "q + 1"} },
                    },
                    Quality = "q",
                    Count = "(r1 <= 0.00025 * p)? 2 : 1",
                    MinutesUntilReady = 600,
                    Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
                },
            };
        }

        private static MachineInformation GetTank()
        {
            return new MachineInformation
            {
                ID = TankID,
                ResourceIndex = 2,
                ResourceLength = 2,
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
                Output = new MachineOutputInformation
                {
                    ID = ItemID.Sugar,
                    Items = new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                    {
                        { ItemID.Beet, new OutputItem() },
                    },
                    Name = "{1} {0}",
                    Price = "p + 25",
                    Quality = "q",
                    Count = "(r1 <= 0.1 * q)? 2 : 1",
                    MinutesUntilReady = 360,
                    Sounds = new List<Sound> { Sound.Ship, Sound.Bubbles },
                },
            };
        }

        private static MachineInformation GetMill()
        {
            return new MachineInformation
            {
                ID = MillID,
                ResourceIndex = 0,
                ResourceLength = 2,
                Name = "Mill",
                Description = "Small machine, that crushes seeds to flour.",
                Skill = Skill.Farming,
                SkillLevel = 7,
                Materials = new Dictionary<DynamicID<ItemID>, int>
                {
                    {ItemID.Wood, 50},
                    {ItemID.Hardwood, 5},
                    {ItemID.IronBar, 1},
                },
                Output = new MachineOutputInformation
                {
                    ID = ItemID.WheatFlour,
                    Items = new Dictionary<DynamicID<ItemID, CategoryID>, OutputItem>
                    {
                        {ItemID.Wheat, new OutputItem()},
                        {ItemID.Amaranth, new OutputItem()},
                        {ItemID.Corn, new OutputItem()},
                    },
                    Name = "{1} {0}",
                    Price = "p + 25",
                    Quality = "q",
                    Count = "(r1 <= 0.1 * q)? 2 : 1",
                    MinutesUntilReady = 360,
                },
                Draw = new MachineDraw
                {
                    Ready = +1,
                }
            };
        }

        private static List<GiftPreferences> GetGiftPreferences()
        {
            return new List<GiftPreferences>
            {
                new GiftPreferences(CharacterName.Penny) { Neutral = new PreferencesList {ColorWineID} },
                new GiftPreferences(CharacterName.Leah) { Loved = new PreferencesList {ColorWineID} },
                new GiftPreferences(CharacterName.Elliott) { Liked = new PreferencesList {ColorWineID} },
                new GiftPreferences(CharacterName.Gus) { Liked = new PreferencesList {ColorWineID} },
                new GiftPreferences(CharacterName.Harvey) { Loved = new PreferencesList {ColorWineID} },
                new GiftPreferences(CharacterName.Jas) { Liked = new PreferencesList {ColorJellyID} },
                new GiftPreferences(CharacterName.Vincent) { Liked = new PreferencesList {ColorJellyID} },
                new GiftPreferences(CharacterName.Maru) { Hated = new PreferencesList {ColorPicklesID} },
                new GiftPreferences(CharacterName.Sam) { Hated = new PreferencesList {ColorPicklesID} },
                new GiftPreferences(CharacterName.Shane) { Hated = new PreferencesList {ColorPicklesID} },
                new GiftPreferences(CharacterName.Harvey) { Loved = new PreferencesList {ColorPicklesID} },
            };
        }

        #endregion
    }
}
