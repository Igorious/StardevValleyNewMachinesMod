using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Igorious.StardewValley.DynamicAPI;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Igorious.StardewValley.NewMachinesMod
{
    public class NewMachinesMod : Mod
    {
        public static NewMachinesModConfig Config { get; private set; }
        private static Task CompilingTask { get; set; }

        public override void Entry(params object[] objects)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Config = new NewMachinesModConfig().Load(Path.Combine(PathOnDisk, @"Configuration"));
            Configurator.PathOnDisk = PathOnDisk;

            InitializeCraftingRecipes();
            InitializeObjectInformation();
            InitializeObjectMapping();
            OverrideTextures();
            PrecompileExpressions();

            PlayerEvents.LoadedGame += (s, e) => CompilingTask?.Wait();
        }

        private static void InitializeObjectMapping()
        {
            ObjectMapper.AddMapping<Mill>(Config.Mill);
            ObjectMapper.AddMapping<Tank>(Config.Tank);
            ObjectMapper.AddMapping<FullTank>(Config.Tank.ID + 1);
            ObjectMapper.AddMapping<VinegarJug>(Config.VinegarJug);
            ObjectMapper.AddMapping<KegEx>(Config.KegEx.ID);
            ObjectMapper.TrackChanges();
        }

        private static void InitializeCraftingRecipes()
        {
            CustomCraftingRecipes.Add(Config.Mill);
            CustomCraftingRecipes.Add(Config.Tank);
            CustomCraftingRecipes.Add(Config.VinegarJug);
        }

        private static void InitializeObjectInformation()
        {
            var craftables = new[] {Config.Mill, Config.Tank, Config.VinegarJug};
            foreach (var craftable in craftables)
            {
                var length = craftable.ResourceLength;
                for (var i = 0; i < length; i++)
                {
                    CustomObjectInformations.AddBigCraftable(craftable.ID + i, craftable.Name, craftable.Description);
                }
            }

            foreach (var overridedItem in Config.ItemOverrides)
            {
                CustomObjectInformations.OverrideItemInformation(overridedItem.ID, overridedItem.Name, overridedItem.Description);
            }

            foreach (var item in Config.Items)
            {
                CustomObjectInformations.AddItem(item);
            }

            foreach (var item in Config.Trees)
            {
                CustomObjectInformations.AddTree(item);
            }

            foreach (var item in Config.Crops)
            {
                CustomObjectInformations.AddCrop(item);
            }
        }

        private static void OverrideTextures()
        {
            var craftables = new[] { Config.Mill, Config.Tank, Config.VinegarJug };
            OverrideTextures(craftables, Textures.AddCraftableOverride);
            OverrideTextures(Config.Items, Textures.AddItemOverride);
            OverrideTextures(Config.Trees, Textures.AddTreeOverride);
            OverrideTextures(Config.Crops, Textures.AddCropOverride);
        }

        private static void OverrideTextures(IEnumerable<IDrawable> items, Action<int, int> addOverride)
        {
            foreach (var item in items)
            {
                var length = item.ResourceLength;
                for (var i = 0; i < length; ++i)
                {
                    if (item.ResourceIndex != null) addOverride(item.TextureIndex + i, item.ResourceIndex.Value + i);
                }
            }
        }

        private static void PrecompileExpressions()
        {
            CompilingTask = Task.Run(() =>
            {
                var machines = new IMachineOutput[] { Config.Mill, Config.Tank, Config.VinegarJug, Config.KegEx };
                foreach (var machine in machines)
                {
                    MachineBase.GetCustomQualityFunc(machine.Output.Quality);
                    MachineBase.GetCustomCountFunc(machine.Output.Count);
                    MachineBase.GetCustomPriceFunc(machine.Output.Price);

                    foreach (var outputItem in machine.Output.Items.Values)
                    {
                        MachineBase.GetCustomCountFunc(outputItem.Count);
                        MachineBase.GetCustomPriceFunc(outputItem.Quality);
                    }
                }
            });
        }
    }
}
