using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Delegates;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.DynamicAPI.Utils;
using Igorious.StardewValley.NewMachinesMod.SmartObjects;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Dynamic;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Igorious.StardewValley.NewMachinesMod
{
    public class NewMachinesMod : Mod
    {
        public static NewMachinesModConfig Config { get; } = new NewMachinesModConfig();

        private List<NewMachinesModConfig.MachineInfo> _machines;

        public override void Entry(params object[] objects)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Config.Load(PathOnDisk);
            _machines = new List<NewMachinesModConfig.MachineInfo>(Config.SimpleMachines)
            {
                Config.Tank,
            };

            InitializeCraftingRecipes();
            InitializeObjectInformation();
            InitializeObjectMapping();
            OverrideTextures();
            PrecompileExpressions();
        }

        private static void InitializeObjectMapping()
        {
            ClassMapperService.Instance.Map<Tank>(Config.Tank.ID);
            ClassMapperService.Instance.Map<FullTank>(Config.Tank.ID + 1);

            Config.SimpleMachines.ForEach(m => ClassMapperService.Instance.Map(DynamicTypeInfo.Create<DynamicCustomMachine>(m.ID)));
            Config.MachineOverrides.ForEach(m => ClassMapperService.Instance.Map(DynamicTypeInfo.Create<DynamicOverridedMachine>(m.ID)));
        }

        private void InitializeCraftingRecipes()
        {
            _machines.Select(m => m.GetCraftingRecipe()).ToList()
                .ForEach(RecipesService.Instance.Register);
        }

        private void InitializeObjectInformation()
        {
            _machines.Select(m => m.GetCraftableInformation()).ToList()
                .ForEach(InformationService.Instance.Register);
            Config.ItemOverrides.ForEach(InformationService.Instance.Override);
            Config.Items.ForEach(InformationService.Instance.Register);
            Config.Crops.ForEach(InformationService.Instance.Register);
        }

        private void OverrideTextures()
        {
            var textureService = new TexturesService(PathOnDisk);
            _machines.ForEach(i => textureService.Override(Texture.Craftables, i));
            Config.Items.ForEach(i => textureService.Override(Texture.Items, i));
            Config.Crops.ForEach(i => textureService.Override(Texture.Crops, i));
        }

        private void PrecompileExpressions()
        {
            var compilingTask = Task.Run(() =>
            {
                var machines = _machines.Concat<IMachineOutput>(Config.MachineOverrides);
                foreach (var machine in machines)
                {
                    var output = machine.Output;
                    ExpressionCompiler.CompileExpression<CountExpression>(output.Count);
                    ExpressionCompiler.CompileExpression<QualityExpression>(output.Quality);
                    ExpressionCompiler.CompileExpression<PriceExpression>(output.Price);

                    foreach (var outputItem in output.Items.Values)
                    {
                        ExpressionCompiler.CompileExpression<CountExpression>(outputItem.Count);
                        ExpressionCompiler.CompileExpression<QualityExpression>(outputItem.Quality);
                    }
                }
            });

            PlayerEvents.LoadedGame += (s, e) => compilingTask?.Wait();
        }
    }
}
