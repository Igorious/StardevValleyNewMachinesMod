using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Objects;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.DynamicAPI.Utils;
using Igorious.StardewValley.NewMachinesMod.Data;
using Igorious.StardewValley.NewMachinesMod.SmartObjects;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Dynamic;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Igorious.StardewValley.NewMachinesMod
{
    public class NewMachinesMod : Mod
    {
        public static NewMachinesModConfig Config { get; } = new NewMachinesModConfig();

        private List<MachineInformation> _machines;

        public override void Entry(params object[] objects)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Config.Load(PathOnDisk);
            _machines = new List<MachineInformation>(Config.SimpleMachines) { Config.Tank, Config.Mixer };

            InitializeCraftingRecipes();
            InitializeObjectInformation();
            InitializeObjectMapping();
            InitializeGiftPreferences();
            InitializeBundles();
            OverrideTextures();
            PrecompileExpressions();
        }

        private static void InitializeGiftPreferences()
        {
            Config.GiftPreferences.ForEach(GiftPreferencesService.Instance.AddGiftPreferences);
        }

        private static void InitializeObjectMapping()
        {
            ClassMapperService.Instance.MapCraftable<Tank>(Config.Tank.ID);
            ClassMapperService.Instance.MapCraftable<FullTank>(Config.Tank.ID + 1);
            ClassMapperService.Instance.MapCraftable<Mixer>(Config.Mixer.ID);

            Config.Items.ForEach(i => ClassMapperService.Instance.MapItem<SmartColoredObject>(i.ID));
            Config.SimpleMachines.ForEach(m => ClassMapperService.Instance.MapCraftable(DynamicTypeInfo.Create<DynamicCustomMachine>(m.ID)));
            Config.MachineOverrides.ForEach(m => ClassMapperService.Instance.MapCraftable(DynamicTypeInfo.Create<DynamicOverridedMachine>(m.ID)));
        }

        private void InitializeCraftingRecipes()
        {
            _machines.ForEach(m => RecipesService.Instance.Register((CraftingRecipeInformation)m));
        }

        private void InitializeObjectInformation()
        {
            _machines.ForEach(m => InformationService.Instance.Register((CraftableInformation)m));
            Config.ItemOverrides.ForEach(InformationService.Instance.Override);
            Config.Items.ForEach(InformationService.Instance.Register);
            Config.Crops.ForEach(InformationService.Instance.Register);
        }

        private void InitializeBundles()
        {
            Config.Bundles.Added.ForEach(BundlesService.Instance.AddBundleItems);
            Config.Bundles.Removed.ForEach(BundlesService.Instance.RemoveBundleItems);
        }

        private void OverrideTextures()
        {
            var textureService = new TexturesService(PathOnDisk);
            _machines.ForEach(i => textureService.Override(Texture.Craftables, i));
            Config.Items.ForEach(i => textureService.Override(Texture.Items, i));
            Config.Crops.ForEach(i => textureService.Override(Texture.Crops, i));
            textureService.OverrideIridiumQualityStar();
        }

        private void PrecompileExpressions()
        {
            var compilingTask = Task.Run(() =>
            {
                var machineOutputs = _machines.Select(m => m.Output).Concat(Config.MachineOverrides.Select(m => m.Output));
                foreach (var machineOutput in machineOutputs)
                {
                    ExpressionCompiler.CompileExpression<CountExpression>(machineOutput.Count);
                    ExpressionCompiler.CompileExpression<QualityExpression>(machineOutput.Quality);
                    ExpressionCompiler.CompileExpression<PriceExpression>(machineOutput.Price);

                    foreach (var outputItem in machineOutput.Items.Values)
                    {
                        if (outputItem == null) continue;
                        ExpressionCompiler.CompileExpression<CountExpression>(outputItem.Count);
                        ExpressionCompiler.CompileExpression<QualityExpression>(outputItem.Quality);
                        ExpressionCompiler.CompileExpression<QualityExpression>(outputItem.Price);
                    }
                }
            });

            PlayerEvents.LoadedGame += (s, e) => compilingTask?.Wait();
        }
    }
}
