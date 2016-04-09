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
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Igorious.StardewValley.NewMachinesMod
{
    public class NewMachinesMod : Mod
    {
        public static NewMachinesModConfig Config { get; } = new NewMachinesModConfig();

        public override void Entry(params object[] objects)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Config.Load(PathOnDisk);

            InitializeCraftingRecipes();
            InitializeObjectInformation();
            InitializeObjectMapping();
            OverrideTextures();
            PrecompileExpressions();
        }

        private static void InitializeObjectMapping()
        {
            ClassMapperService.Instance.Map<Mill>(Config.Mill.ID);
            ClassMapperService.Instance.Map<Tank>(Config.Tank.ID);
            ClassMapperService.Instance.Map<FullTank>(Config.Tank.ID + 1);
            ClassMapperService.Instance.Map<VinegarJug>(Config.VinegarJug.ID);
            ClassMapperService.Instance.Map<KegEx>(Config.KegEx.ID);
        }

        private static void InitializeCraftingRecipes()
        {
            CraftingRecipesService.Instance.Register(Config.Mill);
            CraftingRecipesService.Instance.Register(Config.Tank);
            CraftingRecipesService.Instance.Register(Config.VinegarJug);
        }

        private static void InitializeObjectInformation()
        {
            var craftableInformation = new[] {Config.Mill, Config.Tank, Config.VinegarJug}
            .Select(i => new CraftableInformation
            {
                ID = i.ID,
                Name = i.Name,
                Description = i.Description,
            }).ToList();

            craftableInformation.ForEach(InformationService.Instance.Register);
            Config.ItemOverrides.ForEach(InformationService.Instance.Override);
            Config.Items.ForEach(InformationService.Instance.Register);
        }

        private void OverrideTextures()
        {
            var textureService = new TexturesService(PathOnDisk);
            var craftables = new List<IDrawable> { Config.Mill, Config.Tank, Config.VinegarJug };
            craftables.ForEach(i => textureService.Override(Texture.Craftables, i));
            Config.Items.ForEach(i => textureService.Override(Texture.Items, i));
        }

        private static void PrecompileExpressions()
        {
            var compilingTask = Task.Run(() =>
            {
                var machines = new IMachineOutput[] { Config.Mill, Config.Tank, Config.VinegarJug, Config.KegEx };
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
