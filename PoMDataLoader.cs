using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using PathOfModifiers.Rarities;
using PathOfModifiers.Maps;
using PathOfModifiers.Maps.Generators;

namespace PathOfModifiers
{
    public static class PoMDataLoader
    {
        public static List<Mod> mods = new List<Mod>();

        public static Dictionary<Type, int> affixItemMap;
        public static AffixesItem.Affix[] affixesItem = new AffixesItem.Affix[0];
        public static Dictionary<Type, int> rarityItemMap;
        public static RarityItem[] raritiesItem = new RarityItem[0];

        public static Dictionary<Type, int> affixNPCMap;
        public static AffixesNPC.Affix[] affixesNPC = new AffixesNPC.Affix[0];
        public static Dictionary<Type, int> rarityNPCMap;
        public static RarityNPC[] raritiesNPC = new RarityNPC[0];

        public static Dictionary<Type, int> generatorMap;
        public static Generator[] generators = new Generator[0];
        public static Dictionary<Type, int> mapMap;
        public static Map[] maps = new Map[0];

        public static void Initialize()
        {
            LoadData();
        }

        public static void RegisterMod(Mod mod)
        {
            mods.Add(mod);
        }
        /// <summary>
        /// Loads affixes and rarities from loaded mods
        /// </summary>
        public static void LoadData()
        {
            affixItemMap = new Dictionary<Type, int>();
            List<AffixesItem.Affix> affixItemList = new List<AffixesItem.Affix>();

            rarityItemMap = new Dictionary<Type, int>();
            List<RarityItem> rarityItemList = new List<RarityItem>();

            affixNPCMap = new Dictionary<Type, int>();
            List<AffixesNPC.Affix> affixNPCList = new List<AffixesNPC.Affix>();

            rarityNPCMap = new Dictionary<Type, int>();
            List<RarityNPC> rarityNPCList = new List<RarityNPC>();

            generatorMap = new Dictionary<Type, int>();
            List<Generator> generatorList = new List<Generator>();

            mapMap = new Dictionary<Type, int>();
            List<Map> mapList = new List<Map>();

            int affixItemIndex = 0;
            int rarityItemIndex = 0;
            int affixNPCIndex = 0;
            int rarityNPCIndex = 0;
            int mapIndex = 0;
            int generatorIndex = 0;
            AffixesItem.Affix affixItem;
            RarityItem rarityItem;
            AffixesNPC.Affix affixNPC;
            RarityNPC rarityNPC;
            Generator generator;
            Map map;
            foreach (Mod mod in mods)
            {
                var types = mod.Code.GetTypes().Where(t => t.IsClass && !t.IsAbstract);

                foreach (Type t in types)
                {
                    if (t.IsDefined(typeof(ExcludeFromLoadingInPoM), false))
                    {
                        continue;
                    }
                    if (t.IsSubclassOf(typeof(AffixesItem.Affix)) || t == typeof(AffixesItem.Affix))
                    {
                        affixItem = (AffixesItem.Affix)Activator.CreateInstance(t);
                        affixItem.mod = mod;
                        affixItemList.Add(affixItem);
                        affixItemMap.Add(t, affixItemIndex);
                        mod.Logger.Debug($"Added item affix {t.FullName} with index {affixItemIndex} from mod {mod.Name}");
                        affixItemIndex++;
                    }
                    else if (t.IsSubclassOf(typeof(RarityItem)) && t != typeof(RarityItem))
                    {
                        rarityItem = (RarityItem)Activator.CreateInstance(t);
                        rarityItem.mod = mod;
                        rarityItemList.Add(rarityItem);
                        rarityItemMap.Add(t, rarityItemIndex);
                        mod.Logger.Debug($"Added item rarity {t.FullName} with index {rarityItemIndex} from mod {mod.Name}");
                        rarityItemIndex++;
                    }
                    else if (t.IsSubclassOf(typeof(AffixesNPC.Affix)) || t == typeof(AffixesNPC.Affix))
                    {
                        affixNPC = (AffixesNPC.Affix)Activator.CreateInstance(t);
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
            affixesItem = affixItemList.ToArray();
            raritiesItem = rarityItemList.ToArray();
            affixesNPC = affixNPCList.ToArray();
            raritiesNPC = rarityNPCList.ToArray();
            generators = generatorList.ToArray();
            maps = mapList.ToArray();
        }
        public static void Unload()
        {
            mods = new List<Mod>();

            affixItemMap = new Dictionary<Type, int>();
            affixesItem = new AffixesItem.Affix[0];

            rarityItemMap = new Dictionary<Type, int>();
            raritiesItem = new RarityItem[0];

            affixNPCMap = new Dictionary<Type, int>();
            affixesNPC = new AffixesNPC.Affix[0];

            rarityNPCMap = new Dictionary<Type, int>();
            raritiesNPC = new RarityNPC[0];

            generatorMap = new Dictionary<Type, int>();
            generators = new Generator[0];

            mapMap = new Dictionary<Type, int>();
            maps = new Map[0];
        }

        public static void SendMaps(ModPacket packet)
        {
            packet.Write(affixesItem.Length);
            PathOfModifiers.Instance.Logger.Debug($"Sending Item Affix Map Length: {affixesItem.Length} ");
            AffixesItem.Affix affixItem;
            for (int i = 0; i < affixesItem.Length; i++)
            {
                affixItem = affixesItem[i];
                PathOfModifiers.Instance.Logger.Debug($"SendMaps: {i} / {affixItem.GetType().FullName} from mod {affixItem.mod}");
                packet.Write(affixItem.mod.Name);
                packet.Write(affixItem.GetType().FullName);
            }

            packet.Write(raritiesItem.Length);
            PathOfModifiers.Instance.Logger.Debug($"Sending Item Rarity Map Length: {raritiesItem.Length} ");
            RarityItem rarityItem;
            for (int i = 0; i < raritiesItem.Length; i++)
            {
                rarityItem = raritiesItem[i];
                packet.Write(rarityItem.mod.Name);
                packet.Write(rarityItem.GetType().FullName);
            }

            packet.Write(affixesNPC.Length);
            PathOfModifiers.Instance.Logger.Debug($"Sending NPC Affix Map Length: {affixesNPC.Length} ");
            AffixesNPC.Affix affixNPC;
            for (int i = 0; i < affixesNPC.Length; i++)
            {
                affixNPC = affixesNPC[i];
                PathOfModifiers.Instance.Logger.Debug($"SendMaps: {i} / {affixNPC.GetType().FullName} from mod {affixNPC.mod}");
                packet.Write(affixNPC.mod.Name);
                packet.Write(affixNPC.GetType().FullName);
            }

            packet.Write(raritiesNPC.Length);
            PathOfModifiers.Instance.Logger.Debug($"Sending NPC Rarity Map Length: {raritiesNPC.Length} ");
            RarityNPC rarityNPC;
            for (int i = 0; i < raritiesNPC.Length; i++)
            {
                rarityNPC = raritiesNPC[i];
                packet.Write(rarityNPC.mod.Name);
                packet.Write(rarityNPC.GetType().FullName);
            }

            packet.Write(generators.Length);
            Generator generator;
            for (int i = 0; i < generators.Length; i++)
            {
                generator = generators[i];
                packet.Write(generator.mod.Name);
                packet.Write(generator.GetType().FullName);
            }

            packet.Write(maps.Length);
            Map map;
            for (int i = 0; i < maps.Length; i++)
            {
                map = maps[i];
                packet.Write(map.mod.Name);
                packet.Write(map.GetType().FullName);
            }
        }
        public static void ReceiveDataMaps(BinaryReader reader)
        {
            try
            {
                #region Item Affixes
                int length = reader.ReadInt32();
                PathOfModifiers.Instance.Logger.Debug($"Receiving Item Affix Map Length: {length} ");
                PathOfModifiers.Instance.Logger.Debug($"Loaded Item Affix Map Length: {affixItemMap.Count} ");

                Dictionary<Type, int> newAffixItemMap = new Dictionary<Type, int>(length);
                AffixesItem.Affix[] newAffixesItem = new AffixesItem.Affix[length];

                Type type;
                Mod mod;
                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

                    PathOfModifiers.Instance.Logger.Debug($"ReceiveMaps: {i} / {type.FullName} from mod {mod}");

                    newAffixesItem[i] = affixesItem[affixItemMap[type]];
                    newAffixItemMap.Add(type, i);
                }

                affixItemMap = newAffixItemMap;
                affixesItem = newAffixesItem;
                #endregion
                #region Item Rarities
                length = reader.ReadInt32();
                PathOfModifiers.Instance.Logger.Debug($"Receiving Item Rarity Map Length: {length} ");
                PathOfModifiers.Instance.Logger.Debug($"Loaded Item Rarity Map Length: {rarityItemMap.Count} ");

                Dictionary<Type, int> newRarityMapItem = new Dictionary<Type, int>(length);
                RarityItem[] newRaritiesItem = new RarityItem[length];

                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

                    newRaritiesItem[i] = raritiesItem[rarityItemMap[type]];
                    newRarityMapItem.Add(type, i);
                }

                rarityItemMap = newRarityMapItem;
                raritiesItem = newRaritiesItem;
                #endregion
                #region NPC Affixes
                length = reader.ReadInt32();
                PathOfModifiers.Instance.Logger.Debug($"Receiving NPC Affix Map Length: {length} ");
                PathOfModifiers.Instance.Logger.Debug($"Loaded NPC Affix Map Length: {affixNPCMap.Count} ");

                Dictionary<Type, int> newAffixNPCMap = new Dictionary<Type, int>(length);
                AffixesNPC.Affix[] newAffixesNPC = new AffixesNPC.Affix[length];

                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

                    PathOfModifiers.Instance.Logger.Debug($"ReceiveMaps: {i} / {type.FullName} from mod {mod}");

                    newAffixesNPC[i] = affixesNPC[affixNPCMap[type]];
                    newAffixNPCMap.Add(type, i);
                }

                affixNPCMap = newAffixNPCMap;
                affixesNPC = newAffixesNPC;
                #endregion
                #region NPC Rarities
                length = reader.ReadInt32();
                PathOfModifiers.Instance.Logger.Debug($"Receiving NPC Rarity Map Length: {length} ");
                PathOfModifiers.Instance.Logger.Debug($"Loaded NPC Rarity Map Length: {rarityNPCMap.Count} ");

                Dictionary<Type, int> newRarityNPCMap = new Dictionary<Type, int>(length);
                RarityNPC[] newRaritiesNPC = new RarityNPC[length];

                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

                    newRaritiesNPC[i] = raritiesNPC[rarityNPCMap[type]];
                    newRarityNPCMap.Add(type, i);
                }

                rarityNPCMap = newRarityNPCMap;
                raritiesNPC = newRaritiesNPC;
                #endregion
                #region Map Generators
                length = reader.ReadInt32();

                Dictionary<Type, int> newGenratorMap = new Dictionary<Type, int>(length);
                Generator[] newGenerators = new Generator[length];

                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

                    newGenerators[i] = generators[generatorMap[type]];
                    newGenratorMap.Add(type, i);
                }
                #endregion
                #region Maps
                length = reader.ReadInt32();

                Dictionary<Type, int> newMapMap = new Dictionary<Type, int>(length);
                Map[] newMaps = new Map[length];

                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

                    newMaps[i] = maps[mapMap[type]];
                    newMapMap.Add(type, i);
                }

                mapMap = newMapMap;
                maps = newMaps;
                #endregion
            }
            catch (Exception e)
            {
                PathOfModifiers.Instance.Logger.Fatal(e.ToString());
            }
        }
    }
}
