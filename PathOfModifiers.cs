using PathOfModifiers.Maps;
using PathOfModifiers.Maps.Generators;
using PathOfModifiers.Rarities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace PathOfModifiers
{
    public class PathOfModifiers : Mod
    {
        public static PathOfModifiers Instance { get; private set; }
        public PathOfModifiers() { }

        public static List<Mod> registeredMods = new();
        public static void RegisterMod(Mod mod)
        {
            registeredMods.Add(mod);
        }

        public struct PoMModData
        {
            public Dictionary<Type, int> affixItemMap;
            public Affixes.Items.Affix[] affixesItem;
            public Dictionary<Type, int> rarityItemMap;
            public RarityItem[] raritiesItem;

            public Dictionary<Type, int> affixNPCMap;
            public Affixes.NPCs.Affix[] affixesNPC;
            public Dictionary<Type, int> rarityNPCMap;
            public RarityNPC[] raritiesNPC;

            public Dictionary<Type, int> generatorMap;
            public Generator[] generators;
            public Dictionary<Type, int> mapMap;
            public Map[] maps;
        }
        /// <summary>
        /// Loads affixes and rarities from registered mods
        /// </summary>
        static PoMModData LoadData()
        {
            Dictionary<Type, int> affixItemMap = new();
            List<Affixes.Items.Affix> affixItemList = new();

            Dictionary<Type, int> rarityItemMap = new();
            List<RarityItem> rarityItemList = new();

            Dictionary<Type, int> affixNPCMap = new();
            List<Affixes.NPCs.Affix> affixNPCList = new();

            Dictionary<Type, int> rarityNPCMap = new();
            List<RarityNPC> rarityNPCList = new();

            Dictionary<Type, int> generatorMap = new();
            List<Generator> generatorList = new();

            Dictionary<Type, int> mapMap = new();
            List<Map> mapList = new();

            int affixItemIndex = 0;
            int rarityItemIndex = 0;
            int affixNPCIndex = 0;
            int rarityNPCIndex = 0;
            int mapIndex = 0;
            int generatorIndex = 0;
            Affixes.Items.Affix affixItem;
            RarityItem rarityItem;
            Affixes.NPCs.Affix affixNPC;
            RarityNPC rarityNPC;
            Generator generator;
            Map map;
            foreach (Mod mod in registeredMods)
            {
                var types = mod.Code.GetTypes().Where(t => t.IsClass && !t.IsAbstract);

                foreach (Type t in types)
                {
                    if (t.IsDefined(typeof(DisableAffix), false))
                    {
                        continue;
                    }
                    if (t.IsSubclassOf(typeof(Affixes.Items.Affix)) || t == typeof(Affixes.Items.Affix))
                    {
                        affixItem = (Affixes.Items.Affix)Activator.CreateInstance(t);
                        affixItem.mod = mod;
                        affixItemList.Add(affixItem);
                        affixItemMap.Add(t, affixItemIndex);
                        //TODO:: Wont this log from the other mod. Should be logged from PoM, check all loggers
                        mod.Logger.Debug($"Added item affix {t.FullName} with index {affixItemIndex} from mod {mod.Name}");
                        affixItemIndex++;
                    }
                    else if (t.IsSubclassOf(typeof(RarityItem)) && t != typeof(RarityItem))
                    {
                        rarityItem = (RarityItem)Activator.CreateInstance(t, mod);
                        rarityItemList.Add(rarityItem);
                        rarityItemMap.Add(t, rarityItemIndex);
                        mod.Logger.Debug($"Added item rarity {t.FullName} with index {rarityItemIndex} from mod {mod.Name}");
                        rarityItemIndex++;
                    }
                    else if (t.IsSubclassOf(typeof(Affixes.NPCs.Affix)) || t == typeof(Affixes.NPCs.Affix))
                    {
                        affixNPC = (Affixes.NPCs.Affix)Activator.CreateInstance(t);
                        affixNPC.mod = mod;
                        affixNPCList.Add(affixNPC);
                        affixNPCMap.Add(t, affixNPCIndex);
                        mod.Logger.Debug($"Added NPC affix {t.FullName} with index {affixNPCIndex} from mod {mod.Name}");
                        affixNPCIndex++;
                    }
                    else if (t.IsSubclassOf(typeof(RarityNPC)) && t != typeof(RarityNPC))
                    {
                        rarityNPC = (RarityNPC)Activator.CreateInstance(t);
                        rarityNPC.mod = mod;
                        rarityNPCList.Add(rarityNPC);
                        rarityNPCMap.Add(t, rarityNPCIndex);
                        mod.Logger.Debug($"Added NPC rarity {t.FullName} with index {rarityNPCIndex} from mod {mod.Name}");
                        rarityNPCIndex++;
                    }
                    else if (t.IsSubclassOf(typeof(Generator)) && t != typeof(Generator))
                    {
                        generator = (Generator)Activator.CreateInstance(t);
                        generator.mod = mod;
                        generatorList.Add(generator);
                        generatorMap.Add(t, generatorIndex);
                        mod.Logger.Debug($"Added generator {t.FullName} with index {generatorIndex} from mod {mod.Name}");
                        generatorIndex++;
                    }
                    else if (t.IsSubclassOf(typeof(Map)) && t != typeof(Map))
                    {
                        map = (Map)Activator.CreateInstance(t);
                        map.mod = mod;
                        map.Initialize();
                        mapList.Add(map);
                        mapMap.Add(t, mapIndex);
                        mod.Logger.Debug($"Added map {t.FullName} with index {mapIndex} from mod {mod.Name}");
                        mapIndex++;
                    }
                }
            }

            return new PoMModData
            {
                affixItemMap = affixItemMap,
                affixesItem = affixItemList.ToArray(),

                rarityItemMap = rarityItemMap,
                raritiesItem = rarityItemList.ToArray(),

                affixNPCMap = affixNPCMap,
                affixesNPC = affixNPCList.ToArray(),

                rarityNPCMap = rarityNPCMap,
                raritiesNPC = rarityNPCList.ToArray(),

                generatorMap = generatorMap,
                generators = generatorList.ToArray(),

                mapMap = mapMap,
                maps = mapList.ToArray(),
            };
        }

        public override void Load()
        {
            Instance = this;

            ModNet.ModNet.Initialize();
            RegisterMod(this);
        }
        public override void PostSetupContent()
        {
            DataManager.Load(LoadData());
            ModContent.GetInstance<Affixes.Items.ItemItem>().PostLoad();
            ModContent.GetInstance<Affixes.NPCs.NPCNPC>().PostLoad();

            //if (Main.netMode != 2)
            //{
            //    new ModifierForgeUI().Initialize();
            //    modifierForgeUI = new UserInterface();
            //    ModifierForgeUI.HideUI();

            //    new MapDeviceUI().Initialize();
            //    mapDeviceUI = new UserInterface();
            //    MapDeviceUI.HideUI();
            //}
        }
        public override void Unload()
        {
            //modifierForgeUI = null;
            //mapDeviceUI = null;
            DataManager.Unload();

            Instance = null;
        }

        public override void AddRecipeGroups()
        {
            RecipeGroup group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Copper Bar", new int[]
            {
            ItemID.CopperBar,
            ItemID.TinBar
            });
            RecipeGroup.RegisterGroup("PathOfModifiers:CopperBar", group);

            group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Silver Bar", new int[]
            {
            ItemID.SilverBar,
            ItemID.TungstenBar
            });
            RecipeGroup.RegisterGroup("PathOfModifiers:SilverBar", group);

            group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Gold Bar", new int[]
            {
            ItemID.GoldBar,
            ItemID.PlatinumBar
            });
            RecipeGroup.RegisterGroup("PathOfModifiers:GoldBar", group);
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            ModNet.ModNet.HandlePacket(reader, whoAmI);
        }
    }
}