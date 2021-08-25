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
using PathOfModifiers.Maps.Generators;

namespace PathOfModifiers
{
    public static class DataManager
    {
        public static class Item
        {
            static Dictionary<Type, int> affixMap;
            static Affixes.Items.Affix[] affixes = new Affixes.Items.Affix[0];
            static Dictionary<Type, int> rarityMap;
            static RarityItem[] rarities = new RarityItem[0];

            #region System
            public static void Load(PathOfModifiers.PoMModData loadedData)
            {
                affixMap = loadedData.affixItemMap;
                affixes = loadedData.affixesItem;

                rarityMap = loadedData.rarityItemMap;
                rarities = loadedData.raritiesItem;
            }
            public static void Unload()
            {
                affixMap = null;
                affixes = null;

                rarityMap = null;
                rarities = null;
            }
            public static void SendDataMaps(ModPacket packet)
            {
                packet.Write(affixes.Length);
                PathOfModifiers.Instance.Logger.Debug($"Sending Item Affix Map Length: {affixes.Length} ");
                Affixes.Items.Affix affixItem;
                for (int i = 0; i < affixes.Length; i++)
                {
                    affixItem = affixes[i];
                    PathOfModifiers.Instance.Logger.Debug($"SendMaps: {i} / {affixItem.GetType().FullName} from mod {affixItem.mod}");
                    packet.Write(affixItem.mod.Name);
                    packet.Write(affixItem.GetType().FullName);
                }

                packet.Write(rarities.Length);
                PathOfModifiers.Instance.Logger.Debug($"Sending Item Rarity Map Length: {rarities.Length} ");
                RarityItem rarityItem;
                for (int i = 0; i < rarities.Length; i++)
                {
                    rarityItem = rarities[i];
                    packet.Write(rarityItem.mod.Name);
                    packet.Write(rarityItem.GetType().FullName);
                }
            }
            public static void ReceiveDataMaps(BinaryReader reader)
            {
                try
                {
                    int length;
                    Type type;
                    Mod mod;

                    #region Item Affixes
                    length = reader.ReadInt32();
                    PathOfModifiers.Instance.Logger.Debug($"Receiving Item Affix Map Length: {length} ");
                    PathOfModifiers.Instance.Logger.Debug($"Loaded Item Affix Map Length: {affixMap.Count} ");

                    Dictionary<Type, int> newAffixItemMap = new Dictionary<Type, int>(length);
                    Affixes.Items.Affix[] newAffixesItem = new Affixes.Items.Affix[length];

                    for (int i = 0; i < length; i++)
                    {
                        string modString = reader.ReadString();
                        string typeString = reader.ReadString();
                        mod = ModLoader.GetMod(modString);
                        type = mod.Code.GetType(typeString, true);

                        PathOfModifiers.Instance.Logger.Debug($"ReceiveMaps: {i} / {type.FullName} from mod {mod}");

                        newAffixesItem[i] = affixes[affixMap[type]];
                        newAffixItemMap.Add(type, i);
                    }

                    affixMap = newAffixItemMap;
                    affixes = newAffixesItem;
                    #endregion
                    #region Item Rarities
                    length = reader.ReadInt32();
                    PathOfModifiers.Instance.Logger.Debug($"Receiving Item Rarity Map Length: {length} ");
                    PathOfModifiers.Instance.Logger.Debug($"Loaded Item Rarity Map Length: {rarityMap.Count} ");

                    Dictionary<Type, int> newRarityMapItem = new Dictionary<Type, int>(length);
                    RarityItem[] newRaritiesItem = new RarityItem[length];

                    for (int i = 0; i < length; i++)
                    {
                        mod = ModLoader.GetMod(reader.ReadString());
                        type = mod.Code.GetType(reader.ReadString(), true);

                        newRaritiesItem[i] = rarities[rarityMap[type]];
                        newRarityMapItem.Add(type, i);
                    }

                    rarityMap = newRarityMapItem;
                    rarities = newRaritiesItem;
                    #endregion
                }
                catch (Exception e)
                {
                    PathOfModifiers.Instance.Logger.Fatal(e.ToString());
                }
            }
            #endregion
            #region Data access
            public static Affixes.Items.Affix[] GetAllAffixesRef()
            {
                return affixes.ToArray();
            }

            public static int GetAffixIndex(Type type)
            {
                return affixMap[type];
            }
            public static Affixes.Items.Affix GetNewAffix(int index)
            {
                Affixes.Items.Affix affix = affixes[index];
                affix = affix.Clone();
                (affix as Affixes.Items.AffixTiered)?.RollValue();
                return affix;
            }
            public static Affixes.Items.Affix GetNewAffix(Type type)
            {
                return GetNewAffix(GetAffixIndex(type));
            }

            public static int GetRarityIndex(Type type)
            {
                return rarityMap[type];
            }
            public static RarityItem GetRarityRef(int index)
            {
                return rarities[index];
            }
            public static RarityItem GetRarityRef(Type type)
            {
                return GetRarityRef(GetRarityIndex(type));
            }
            #endregion
            #region Actions
            /// <summary>
            /// Returns a valid rarity for the item.
            /// </summary>
            public static RarityItem RollRarity(Terraria.Item item)
            {
                Tuple<RarityItem, double>[] tuples = rarities
                    .Where(r => r.Weight > 0 && r.CanBeRolled(item))
                    .Select(r => new Tuple<RarityItem, double>(r, r.Weight))
                    .ToArray();
                WeightedRandom<RarityItem> weightedRandom = new(Main.rand, tuples);
                RarityItem rarity = weightedRandom;
                return rarity;
            }
            public static bool TryRollNewAffix(Affixes.Items.ItemItem pomItem, Terraria.Item item, out Affixes.Items.Affix affix)
            {
                float prefixChance = pomItem.FreeSuffixes > 0 ? pomItem.rarity.chanceToRollPrefixInsteadOfSuffix : 1;

                Affixes.Items.Constraints.Constraint constraint;
                if (Main.rand.NextFloat(1) < prefixChance)
                {
                    constraint = new Affixes.Items.Constraints.Prefixes();
                }
                else
                {
                    constraint = new Affixes.Items.Constraints.Suffixes();
                }

                return TryRollNewAffix(pomItem, item, constraint, out affix);
            }
            public static bool TryRollNewAffix(Affixes.Items.ItemItem pomItem, Terraria.Item item, Affixes.Items.Constraints.Constraint constraint, out Affixes.Items.Affix affix)
            {
                var constrainedAffixes = affixes
                    .Where(a => a.AffixSpaceAvailable(pomItem) &&
                        a.Weight > 0 &&
                        a.CanRoll(pomItem, item) &&
                        !pomItem.affixes.Exists(ia => ia.GetType() == a.GetType()));
                constrainedAffixes = constraint.Process(constrainedAffixes);

                var tuples = constrainedAffixes.Select(a => new Tuple<Affixes.Items.Affix, double>(a, a.Weight)).ToArray();
                if (tuples.Length == 0)
                {
                    affix = null;
                    return false;
                }
                WeightedRandom<Affixes.Items.Affix> weightedRandom = new(Main.rand, tuples);
                affix = weightedRandom.Get().Clone();
                (affix as Affixes.Items.AffixTiered)?.RollValue();

                return true;
            }
            #endregion
        }
        public static class NPC
        {
            static Dictionary<Type, int> affixMap;
            static Affixes.NPCs.Affix[] affixes = new Affixes.NPCs.Affix[0];
            static Dictionary<Type, int> rarityMap;
            static RarityNPC[] rarities = new RarityNPC[0];

            #region System
            public static void Load(PathOfModifiers.PoMModData loadedData)
            {
                affixMap = loadedData.affixNPCMap;
                affixes = loadedData.affixesNPC;

                rarityMap = loadedData.rarityNPCMap;
                rarities = loadedData.raritiesNPC;
            }
            public static void Unload()
            {
                affixMap = null;
                affixes = null;

                rarityMap = null;
                rarities = null;
            }
            public static void SendDataMaps(ModPacket packet)
            {
                packet.Write(affixes.Length);
                PathOfModifiers.Instance.Logger.Debug($"Sending NPC Affix Map Length: {affixes.Length} ");
                Affixes.NPCs.Affix affixNPC;
                for (int i = 0; i < affixes.Length; i++)
                {
                    affixNPC = affixes[i];
                    PathOfModifiers.Instance.Logger.Debug($"SendMaps: {i} / {affixNPC.GetType().FullName} from mod {affixNPC.mod}");
                    packet.Write(affixNPC.mod.Name);
                    packet.Write(affixNPC.GetType().FullName);
                }

                packet.Write(rarities.Length);
                PathOfModifiers.Instance.Logger.Debug($"Sending NPC Rarity Map Length: {rarities.Length} ");
                RarityNPC rarityNPC;
                for (int i = 0; i < rarities.Length; i++)
                {
                    rarityNPC = rarities[i];
                    packet.Write(rarityNPC.mod.Name);
                    packet.Write(rarityNPC.GetType().FullName);
                }
            }
            public static void ReceiveDataMaps(BinaryReader reader)
            {
                try
                {
                    int length;
                    Type type;
                    Mod mod;

                    #region NPC Affixes
                    length = reader.ReadInt32();
                    PathOfModifiers.Instance.Logger.Debug($"Receiving NPC Affix Map Length: {length} ");
                    PathOfModifiers.Instance.Logger.Debug($"Loaded NPC Affix Map Length: {affixMap.Count} ");

                    Dictionary<Type, int> newAffixNPCMap = new Dictionary<Type, int>(length);
                    Affixes.NPCs.Affix[] newAffixesNPC = new Affixes.NPCs.Affix[length];

                    for (int i = 0; i < length; i++)
                    {
                        mod = ModLoader.GetMod(reader.ReadString());
                        type = mod.Code.GetType(reader.ReadString(), true);

                        PathOfModifiers.Instance.Logger.Debug($"ReceiveMaps: {i} / {type.FullName} from mod {mod}");

                        newAffixesNPC[i] = affixes[affixMap[type]];
                        newAffixNPCMap.Add(type, i);
                    }

                    affixMap = newAffixNPCMap;
                    affixes = newAffixesNPC;
                    #endregion
                    #region NPC Rarities
                    length = reader.ReadInt32();
                    PathOfModifiers.Instance.Logger.Debug($"Receiving NPC Rarity Map Length: {length} ");
                    PathOfModifiers.Instance.Logger.Debug($"Loaded NPC Rarity Map Length: {rarityMap.Count} ");

                    Dictionary<Type, int> newRarityNPCMap = new Dictionary<Type, int>(length);
                    RarityNPC[] newRaritiesNPC = new RarityNPC[length];

                    for (int i = 0; i < length; i++)
                    {
                        mod = ModLoader.GetMod(reader.ReadString());
                        type = mod.Code.GetType(reader.ReadString(), true);

                        newRaritiesNPC[i] = rarities[rarityMap[type]];
                        newRarityNPCMap.Add(type, i);
                    }

                    rarityMap = newRarityNPCMap;
                    rarities = newRaritiesNPC;
                    #endregion
                }
                catch (Exception e)
                {
                    PathOfModifiers.Instance.Logger.Fatal(e.ToString());
                }
            }
            #endregion
            #region Data access
            public static int GetAffixIndex(Type type)
            {
                return affixMap[type];
            }
            public static Affixes.NPCs.Affix GetNewAffix(int index)
            {
                Affixes.NPCs.Affix affix = affixes[index];
                affix = affix.Clone();
                affix.RollValue();
                return affix;
            }
            public static Affixes.NPCs.Affix GetNewAffix(Type type)
            {
                return GetNewAffix(GetAffixIndex(type));
            }

            public static int GetRarityIndex(Type type)
            {
                return rarityMap[type];
            }
            public static RarityNPC GetRarityRef(int index)
            {
                return rarities[index];
            }
            public static RarityNPC GetRarityRef(Type type)
            {
                return GetRarityRef(GetRarityIndex(type));
            }
            #endregion
            #region Actions
            /// <summary>
            /// Returns a valid rarity for the NPC.
            /// </summary>
            public static RarityNPC RollRarity(Terraria.NPC npc)
            {
                Tuple<RarityNPC, double>[] tuples = rarities
                    .Where(r => r.Weight > 0 && r.CanBeRolled(npc))
                    .Select(r => new Tuple<RarityNPC, double>(r, r.Weight))
                    .ToArray();
                WeightedRandom<RarityNPC> weightedRandom = new WeightedRandom<RarityNPC>(Main.rand, tuples);
                RarityNPC rarity = (RarityNPC)weightedRandom ?? new NPCNone();
                return rarity;
            }
            /// <summary>
            /// Returns a valid affix for the Terraria.NPC or null.
            /// </summary>
            public static Affixes.NPCs.Affix RollNewAffix(Affixes.NPCs.NPCNPC pomNPC, Terraria.NPC npc)
            {
                if (pomNPC.FreeAffixes <= 0)
                {
                    return null;
                }
                Tuple<Affixes.NPCs.Affix, double>[] tuples = affixes
                    .Where(a => a.AffixSpaceAvailable(pomNPC) &&
                        a.Weight > 0 &&
                        a.CanRoll(pomNPC, npc) &&
                        !pomNPC.affixes.Exists(ia => ia.GetType() == a.GetType()))
                    .Select(a => new Tuple<Affixes.NPCs.Affix, double>(a, a.Weight))
                    .ToArray();
                if (tuples.Length == 0)
                {
                    return null;
                }
                WeightedRandom<Affixes.NPCs.Affix> weightedRandom = new WeightedRandom<Affixes.NPCs.Affix>(Main.rand, tuples);
                Affixes.NPCs.Affix affix = weightedRandom;
                affix = affix.Clone();
                affix.InitializeNPC(pomNPC, npc);
                return affix;
            }
            #endregion
        }
        public static class Map
        {
            static Dictionary<Type, int> generatorMap;
            static Generator[] generators;
            static Dictionary<Type, int> mapMap;
            static Maps.Map[] maps;

            #region System
            public static void Load(PathOfModifiers.PoMModData loadedData)
            {
                generatorMap = loadedData.generatorMap;
                generators = loadedData.generators;

                mapMap = loadedData.mapMap;
                maps = loadedData.maps;
            }
            public static void Unload()
            {
                generatorMap = null;
                generators = null;

                mapMap = null;
                maps = null;
            }
            public static void SendDataMaps(ModPacket packet)
            {
                packet.Write(generators.Length);
                Generator generator;
                for (int i = 0; i < generators.Length; i++)
                {
                    generator = generators[i];
                    packet.Write(generator.mod.Name);
                    packet.Write(generator.GetType().FullName);
                }

                packet.Write(maps.Length);
                Maps.Map map;
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
                    int length;
                    Type type;
                    Mod mod;

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
                    Maps.Map[] newMaps = new Maps.Map[length];

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
            #endregion
            #region Data access
            public static int GetGeneratorIndex(Type type)
            {
                return generatorMap[type];
            }
            public static Generator GetGeneratorRef(int index)
            {
                return generators[index];
            }
            public static Generator GetGeneratorRef(Type type)
            {
                return GetGeneratorRef(GetGeneratorIndex(type));
            }

            public static int GetMapIndex(Type type)
            {
                return mapMap[type];
            }
            public static Maps.Map GetNewMap(int index)
            {
                return maps[index].Clone();
            }
            public static Maps.Map GetNewMap(Type type)
            {
                return GetNewMap(GetMapIndex(type));
            }
            #endregion
        }

        public static void Load(PathOfModifiers.PoMModData loadedData)
        {
            Item.Load(loadedData);
            NPC.Load(loadedData);
            Map.Load(loadedData);
        }
        public static void Unload()
        {
            Item.Unload();
            NPC.Unload();
            Map.Unload();
        }
        public static void SendDataMaps(ModPacket packet)
        {
            Item.SendDataMaps(packet);
            NPC.SendDataMaps(packet);
            Map.SendDataMaps(packet);
        }
        public static void ReceiveDataMaps(BinaryReader reader)
        {
            Item.ReceiveDataMaps(reader);
            NPC.ReceiveDataMaps(reader);
            Map.ReceiveDataMaps(reader);
        }
    }
}
