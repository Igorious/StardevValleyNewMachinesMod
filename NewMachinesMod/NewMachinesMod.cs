using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Igorious.StardewValley.DynamicAPI;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.NewMachinesMod.SmartObjects;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Base;
using StardewModdingAPI;
using StardewModdingAPI.Events;

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
            CustomObjectInformations.AddBigCraftable(Config.Mill);
            CustomObjectInformations.AddBigCraftable(Config.Tank);
            CustomObjectInformations.AddBigCraftable(Config.Tank.ID + 1, Config.Tank.Name, Config.Tank.Description);
            CustomObjectInformations.AddBigCraftable(Config.VinegarJug);

            foreach (var overrided in Config.Overrides)
            {
                CustomObjectInformations.OverrideItemInformation(overrided.ID, overrided.Name, overrided.Description);
            }

            foreach (var item in Config.Items)
            {
                CustomObjectInformations.AddItem(item);
            }
        }

        private static void OverrideTextures()
        {
            var xnbIndexes = new[]
            {
                Config.Mill.ID,
                Config.Mill.ID + 1,
                Config.Tank.ID,
                Config.Tank.ID + 1,
                Config.VinegarJug.ID,
            };

            for (var i = 0; i < xnbIndexes.Length; ++i)
            {
                Textures.AddCraftableOverride(xnbIndexes[i], i);
            }

            for (var i = 0; i < Config.Items.Count; ++i)
            {
                Textures.AddItemOverride(Config.Items[i].ID, i);
            }
        }

        private static void PrecompileExpressions()
        {
            CompilingTask = Task.Run(() =>
            {
                var machines = new[] { Config.Mill, Config.Tank, Config.VinegarJug };
                foreach (var machine in machines)
                {
                    MachineBase.GetCustomQualityFunc(machine.Output.Quality);
                    MachineBase.GetCustomCountFunc(machine.Output.Count);
                    MachineBase.GetCustomPriceFunc(machine.Output.Price);
                }
            });
        }
    }
}
