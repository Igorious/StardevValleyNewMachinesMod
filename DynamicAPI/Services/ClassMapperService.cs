using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Igorious.StardewValley.DynamicAPI.Locations;
using Igorious.StardewValley.DynamicAPI.Services.Internal;
using Igorious.StardewValley.DynamicAPI.Utils;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class ClassMapperService
    {
        #region Singleton Access

        private ClassMapperService()
        {
            MapDefaultLocations();

            PlayerEvents.LoadedGame += (s, e) =>
            {
                if (!ActivateMapping()) return;
                Log.ImportantInfo("Activation after loading...");
                ConvertToSmartInWorld();
            };

            LocationEvents.CurrentLocationChanged += BuildableActivation;

            TimeEvents.TimeOfDayChanged += (s, e) =>
            {
                if (Game1.timeOfDay != 610) return;
                if (!ActivateMapping()) return;
                Log.ImportantInfo("FORCED ACTIVATION!");
                ConvertToSmartInWorld();
            };

            TimeEvents.OnNewDay += (s, e) =>
            {
                if (e.IsNewDay) return;
                if (!DeactivateMapping()) return;
                ConvertToRawInWorld(); // Allow use native serializer.
            };

            GameEvents.UpdateTick += (s, e) => ConvertActiveInventoryObject();
            PlayerEvents.InventoryChanged += OnInventoryChanged;

            MenuEvents.MenuChanged += (s, e) =>
            {
                if (!(e.NewMenu is GameMenu)) return;

                CraftingMenu = e.NewMenu.GetField<List<IClickableMenu>>("pages").OfType<CraftingPage>().First();
                GameEvents.UpdateTick += OnCrafted;
                MenuEvents.MenuClosed += OnCraftingMenuClosed;
            };
        }

        private void MapDefaultLocations()
        {
            MapLocation<AdventureGuild, SmartAdventureGuild>();
            MapLocation<AnimalHouse, SmartAnimalHouse>();
            MapLocation<BathHousePool, SmartBathHousePool>();
            MapLocation<Beach, SmartBeach>();
            MapLocation<BusStop, SmartBusStop>();
            MapLocation<Club, SmartClub>();
            MapLocation<CommunityCenter, SmartCommunityCenter>();
            MapLocation<Desert, SmartDesert>();
            MapLocation<Farm, SmartFarm>();
            MapLocation<FarmCave, SmartFarmCave>();
            MapLocation<FarmHouse, SmartFarmHouse>();
            MapLocation<Forest, SmartForest>();
            MapLocation<JojaMart, SmartJojaMart>();
            MapLocation<LibraryMuseum, SmartLibraryMuseum>();
            MapLocation<MineShaft, SmartMineShaft>();
            MapLocation<Mountain, SmartMountain>();
            MapLocation<Railroad, SmartRailroad>();
            MapLocation<SeedShop, SmartSeedShop>();
            MapLocation<Sewer, SmartSewer>();
            MapLocation<SlimeHutch, SmartSlimeHutch>();
            MapLocation<Summit, SmartSummit>();
            MapLocation<Town, SmartTown>();
            MapLocation<WizardHouse, SmartWizardHouse>();
            MapLocation<Woods, SmartWoods>();

            MapLocation<GameLocation, SmartGameLocation>();
        }

        private static ClassMapperService _instance;

        public static ClassMapperService Instance => _instance ?? (_instance = new ClassMapperService());

        #endregion

        #region Private Data

        private bool IsActivated { get; set; }

        private Object PreviousActiveObject { get; set; }

        private readonly Dictionary<Type, Type> _locationTypeMap = new Dictionary<Type, Type>();
        private readonly Dictionary<int, Type> _itemTypeMap = new Dictionary<int, Type>();
        private readonly Dictionary<int, Type> _craftableTypeMap = new Dictionary<int, Type>();
        private readonly Dictionary<int, DynamicTypeInfo> _dynamicCraftableTypeMap = new Dictionary<int, DynamicTypeInfo>();

        #endregion

        #region	Properties

        public IReadOnlyDictionary<int, Type> ItemTypeMap => _itemTypeMap;
        public IReadOnlyDictionary<int, Type> CraftableTypeMap => _craftableTypeMap;
        public IReadOnlyDictionary<int, DynamicTypeInfo> DynamicCraftableTypeMap => _dynamicCraftableTypeMap;

        #endregion

        #region Handlers

        private void OnCraftingMenuClosed(object sender, EventArgsClickableMenuClosed eventArgsClickableMenuClosed)
        {
            GameEvents.UpdateTick -= OnCrafted;
            MenuEvents.MenuClosed -= OnCraftingMenuClosed;
            CraftingMenu = null;
        }

        private CraftingPage CraftingMenu { get; set; }

        private void OnCrafted(object sender, EventArgs e)
        {
            if (CraftingMenu == null || !IsActivated) return;

            var heldItem = CraftingMenu.GetField<Item>("heldItem") as Object;
            if (IsRawObject(heldItem))
            {
                CraftingMenu.SetField<Item>("heldItem", ToSmartObject(heldItem));
            }
        }

        private void ConvertActiveInventoryObject()
        {
            if (!IsActivated) return;

            var activeObject = Game1.player.ActiveObject;
            if (activeObject == PreviousActiveObject) return;

            if (IsRawObject(activeObject))
            {
                Game1.player.ActiveObject = ToSmartObject(activeObject);
            }
            PreviousActiveObject = Game1.player.ActiveObject;
        }

        private void OnInventoryChanged(object sender, EventArgsInventoryChanged args)
        {
            if (!IsActivated || !args.Added.Any()) return;

            var inventory = Game1.player.Items;
            foreach (var addedItemInfo in args.Added)
            {
                var addedItem = addedItemInfo.Item as Object;
                if (!IsRawObject(addedItem)) continue;

                var index = inventory.FindIndex(i => i == addedItem);
                inventory[index] = ToSmartObject(addedItem);
            }
        }

        private void BuildableActivation(object sender, EventArgsCurrentLocationChanged args)
        {
            if (!(args.NewLocation is BuildableGameLocation)) return;
            ConvertToSmartLocations();
        }

        #endregion

        #region	Public Methods

        /// <summary>
        /// Map game Item ID to specific class.
        /// </summary>
        public void MapItem<TObject>(int id) where TObject : ISmartObject, new()
        {
            _itemTypeMap.Add(id, typeof(TObject));
        }

        /// <summary>
        /// Map game Craftable ID to specific class.
        /// </summary>
        public void MapCraftable<TObject>(int id) where TObject : ISmartObject, new()
        {
            _craftableTypeMap.Add(id, typeof(TObject));
        }

        /// <summary>
        /// Map game Craftable ID to specific class.
        /// </summary>
        public void MapCraftable(DynamicTypeInfo dynamicTypeInfo)
        {
            _dynamicCraftableTypeMap.Add(dynamicTypeInfo.ClassID, dynamicTypeInfo);
        }

        /// <summary>
        /// Get mapped Craftable ID for specific class.
        /// </summary>
        public int GetCraftableID<TObject>() where TObject : ISmartObject
        {
            return _craftableTypeMap.First(kv => kv.Value == typeof(TObject)).Key;
        }

        /// <summary>
        /// Get mapped ID for specific class.
        /// </summary>
        public int GetItemID<TObject>() where TObject : ISmartObject
        {
            return _itemTypeMap.First(kv => kv.Value == typeof(TObject)).Key;
        }

        public void MapLocation<TLocationBase, TLocationSmart>() where TLocationSmart : TLocationBase, ISmartLocation where TLocationBase : GameLocation
        {
            _locationTypeMap.Add(typeof(TLocationBase), typeof(TLocationSmart));
        }

        public Object ToSmartObject(Object rawObject)
        {
            var smartObject = rawObject.bigCraftable
                ? CraftableToSmartObject(rawObject)
                : ItemToSmartObject(rawObject);
            Cloner.Instance.CopyData(rawObject, smartObject);
            return smartObject;
        }

        public bool IsRawObject(Object o)
        {
            return IsRawCraftable(o) || IsRawItem(o);
        }

        #endregion

        #region	Auxiliary Methods

        #region Handlers

        private void OnMenuClosed(object sender, EventArgsClickableMenuClosed args)
        {
            if (!(args.PriorMenu is SaveGameMenu)) return;
            if (!ActivateMapping()) return;
            ConvertToSmartInWorld();
        }

        private void OnLocationObjectsChanged(object sender, EventArgsLocationObjectsChanged eventArgsLocationObjectsChanged)
        {
            ConvertToSmartObjectsInLocation();
        }

        #endregion

        #region Conversion

        private Object CraftableToSmartObject(Object rawObject)
        {
            var craftableID = rawObject.ParentSheetIndex;

            Type type;
            if (_craftableTypeMap.TryGetValue(craftableID, out type))
            {
                var ctor = type.GetConstructor(Type.EmptyTypes);
                if (ctor != null) return (Object)ctor.Invoke(new object[] { });
                Log.Error($"Can't find .ctor (static) for CraftableID={craftableID}.");
                return rawObject;
            }

            DynamicTypeInfo dynamicTypeInfo;
            if (_dynamicCraftableTypeMap.TryGetValue(craftableID, out dynamicTypeInfo))
            {
                var ctor = dynamicTypeInfo.BaseType.GetConstructor(new[] { typeof(int) });
                if (ctor != null) return (Object)ctor.Invoke(new object[] { dynamicTypeInfo.ClassID });
                Log.Error($"Can't find .ctor (dynamic) for CraftableID={craftableID}.");
                return rawObject;
            }

            Log.Error($"Can't find mapping for CraftableID={craftableID}.");
            return rawObject;
        }

        private Object ItemToSmartObject(Object rawObject)
        {
            var itemID = rawObject.ParentSheetIndex;
            var type = _itemTypeMap[itemID];
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor != null) return (Object)ctor.Invoke(new object[] { });

            Log.Error($"Can't find .ctor (static) for ItemID={itemID}.");
            return rawObject;
        }

        private void ConvertToSmartObjectsInLocation()
        {
            var sw = Stopwatch.StartNew();
            Log.Info($"Activating objects (changes in {Game1.currentLocation?.Name})...");
            Traverser.Instance.ConvertObjectsInLocation(Game1.currentLocation, IsRawObject, ToSmartObject);
            Game1.getAllFarmers().ForEach(f => Traverser.Instance.ConvertObjectsInInventory(f, IsRawObject, ToSmartObject));
            Log.Info($"Convertion ('smart') finished: {sw.ElapsedMilliseconds} ms");
        }

        private void ConvertToRawInWorld()
        {
            var sw = Stopwatch.StartNew();
            Log.Info("Deactivating objects in world...");
            Traverser.Instance.ConvertLocations(l => l is ISmartLocation, l => ConvertLocation(l, GetRawLocationType));
            foreach (var gameLocation in Game1.locations)
            {
                Traverser.Instance.ConvertObjectsInLocation(gameLocation, IsSmartObject, Cloner.Instance.ToRawObject);
            }
            Game1.getAllFarmers().ForEach(f => Traverser.Instance.ConvertObjectsInInventory(f, IsSmartObject, Cloner.Instance.ToRawObject));
            Log.Info($"Convertion ('raw') finished: {sw.ElapsedMilliseconds} ms");
        }

        private void ConvertToSmartInWorld()
        {
            var sw = Stopwatch.StartNew();
            Log.Info("Activating objects in world...");
            ConvertToSmartLocations();
            foreach (var gameLocation in Game1.locations)
            {
                Traverser.Instance.ConvertObjectsInLocation(gameLocation, IsRawObject, ToSmartObject);
            }
            Game1.getAllFarmers().ForEach(f => Traverser.Instance.ConvertObjectsInInventory(f, IsRawObject, ToSmartObject));
            Log.Info($"Convertion ('smart') finished: {sw.ElapsedMilliseconds} ms");
        }

        private void ConvertToSmartLocations()
        {
            Traverser.Instance.ConvertLocations(l => !(l is ISmartLocation), l => ConvertLocation(l, GetSmartLocationType));
        }

        private Type GetRawLocationType(Type smartLocationType)
        {
            return _locationTypeMap.FirstOrDefault(p => p.Value == smartLocationType).Key;
        }

        private Type GetSmartLocationType(Type rawLocationType)
        {
            Type newType;
            return _locationTypeMap.TryGetValue(rawLocationType, out newType) ? newType : null;
        }

        private GameLocation ConvertLocation(GameLocation oldLocation, Func<Type, Type> getNewType)
        {
            var newType = getNewType(oldLocation.GetType());
            if (newType == null)
            {
                Log.Fail($"Can't find mapping for {oldLocation.GetType()}:{oldLocation.Name}");
                return oldLocation;
            }
            var ctor = newType.GetConstructor(Type.EmptyTypes);
            if (ctor != null)
            {
                var newLocation = (GameLocation)ctor.Invoke(new object[] { });
                Cloner.Instance.CopyData(oldLocation, newLocation, oldLocation.GetType());
                Log.Info($"Converted location {oldLocation.Name}.");
                return newLocation;
            }
            else
            {
                Log.Error($"Can't find {newType.FullName}.ctor for Location={oldLocation.Name}.");
                return oldLocation;
            }
        }

        #endregion

        #region Checks

        private bool IsRawCraftable(Object o)
        {
            if (o == null || !o.bigCraftable) return false;
            var craftableID = o.ParentSheetIndex;

            Type type;
            if (_craftableTypeMap.TryGetValue(craftableID, out type))
            {
                return (o.GetType() != type);
            }

            DynamicTypeInfo dynamicTypeInfo;
            if (_dynamicCraftableTypeMap.TryGetValue(craftableID, out dynamicTypeInfo))
            {
                return (o.GetType() != dynamicTypeInfo.BaseType);
            }

            return false;
        }

        private bool IsRawItem(Object o)
        {
            if (o == null || o.bigCraftable) return false;
            var itemID = o.ParentSheetIndex;

            Type type;
            if (_itemTypeMap.TryGetValue(itemID, out type))
            {
                return (o.GetType() != type);
            }

            return false;
        }

        private bool IsSmartObject(Object o)
        {
            return (_craftableTypeMap.ContainsKey(o.ParentSheetIndex) || _itemTypeMap.ContainsKey(o.ParentSheetIndex) || _dynamicCraftableTypeMap.ContainsKey(o.ParentSheetIndex))
                   && !IsSerializable(o);
        }

        private static bool IsSerializable(Item o)
        {
            return new[] { typeof(Item), typeof(Object), typeof(ColoredObject) }.Contains(o.GetType());
        }

        #endregion

        #region Activation

        private bool ActivateMapping()
        {
            if (IsActivated) return false;
            IsActivated = true;
            LocationEvents.LocationObjectsChanged += OnLocationObjectsChanged;
            MenuEvents.MenuClosed -= OnMenuClosed;
            Log.ImportantInfo("Class mapping activated.");
            return true;
        }

        private bool DeactivateMapping()
        {
            if (!IsActivated) return false;
            IsActivated = false;
            LocationEvents.LocationObjectsChanged -= OnLocationObjectsChanged;
            MenuEvents.MenuClosed += OnMenuClosed; // Prevent issue with crash after finishing bundle.
            Log.ImportantInfo("Class mapping deactivated.");
            return true;
        }

        #endregion

        #endregion
    }
}