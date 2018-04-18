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
using PathOfModifiers.Affixes;
using PathOfModifiers.Rarities;

namespace PathOfModifiers
{
	public static class PoMAffixController
    {
        public static List<Mod> mods = new List<Mod>();

        public static Dictionary<Type, int> affixMap;
        public static Affix[] affixes;
        public static Dictionary<Type, int> rarityMap;
        public static Rarity[] rarities;

        public static Dictionary<Type, int> affixNPCMap;
        public static AffixesNPC.Affix[] affixesNPC;

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
            affixMap = new Dictionary<Type, int>();
            List<Affix> affixList = new List<Affix>();

            rarityMap = new Dictionary<Type, int>();
            List<Rarity> rarityList = new List<Rarity>();


            affixNPCMap = new Dictionary<Type, int>();
            List<AffixesNPC.Affix> affixNPCList = new List<AffixesNPC.Affix>();

            int affixIndex = 0;
            int rarityIndex = 0;
            int affixNPCIndex = 0;
            Affix affix;
            Rarity rarity;
            AffixesNPC.Affix affixNPC;
            foreach (Mod mod in mods)
            {
                var types = mod.Code.GetTypes().Where(t => t.IsClass && !t.IsAbstract);

                foreach(Type t in types)
                {
                    if (t.IsSubclassOf(typeof(Affix)) && t != typeof(Affix) && t != typeof(Prefix) && t != typeof(Suffix))
                    {
                        affix = (Affix)Activator.CreateInstance(t);
                        affix.mod = mod;
                        affixList.Add(affix);
                        affixMap.Add(t, affixIndex);
                        affixIndex++;
                        if (PathOfModifiers.logLoad)
                            PathOfModifiers.Log($"PathOfModifiers: Added affix {t.FullName} with index {affixIndex} from mod {mod.Name}");
                    }
                    else if (t.IsSubclassOf(typeof(Rarity)) && t != typeof(Rarity))
                    {
                        rarity = (Rarity)Activator.CreateInstance(t);
                        rarity.mod = mod;
                        rarityList.Add(rarity);
                        rarityMap.Add(t, rarityIndex);
                        rarityIndex++;
                        if (PathOfModifiers.logLoad)
                            PathOfModifiers.Log($"PathOfModifiers: Added rarity {t.FullName} with index {rarityIndex} from mod {mod.Name}");
                    }
                    else if (t.IsSubclassOf(typeof(AffixesNPC.Affix)) && t != typeof(AffixesNPC.Affix) && t != typeof(AffixesNPC.Prefix) && t != typeof(AffixesNPC.Suffix))
                    {
                        affixNPC = (AffixesNPC.Affix)Activator.CreateInstance(t);
                        affixNPC.mod = mod;
                        affixNPCList.Add(affixNPC);
                        affixNPCMap.Add(t, affixNPCIndex);
                        affixNPCIndex++;
                        if (PathOfModifiers.logLoad)
                            PathOfModifiers.Log($"PathOfModifiers: Added NPCAffix {t.FullName} with index {affixNPCIndex} from mod {mod.Name}");
                    }
                }
            }
            affixes = affixList.ToArray();
            rarities = rarityList.ToArray();
            affixesNPC = affixNPCList.ToArray();
        }
        public static void Unload()
        {
            mods = new List<Mod>();

            affixMap = new Dictionary<Type, int>();
            affixes = new Affix[0];

            rarityMap = new Dictionary<Type, int>();
            rarities = new Rarity[0];

            affixNPCMap = new Dictionary<Type, int>();
            affixesNPC = new AffixesNPC.Affix[0];
        }

        public static void SendMaps(ModPacket packet)
        {
            packet.Write(affixes.Length);
            if (PathOfModifiers.logNetwork)
                PathOfModifiers.Log($"SendMaps: {affixes.Length} ");
            Affix affix;
            for (int i = 0; i < affixes.Length; i++)
            {
                affix = affixes[i];
                if (PathOfModifiers.logNetwork)
                    PathOfModifiers.Log($"SendMaps: {i} / {affix.GetType().FullName} from mod {affix.mod}");
                packet.Write(affix.mod.Name);
                packet.Write(affix.GetType().FullName);
            }

            packet.Write(rarities.Length);
            Rarity rarity;
            for (int i = 0; i < rarities.Length; i++)
            {
                rarity = rarities[i];
                packet.Write(rarity.mod.Name);
                packet.Write(rarity.GetType().FullName);
            }

            packet.Write(affixesNPC.Length);
            AffixesNPC.Affix affixNPC;
            for (int i = 0; i < affixesNPC.Length; i++)
            {
                affixNPC = affixesNPC[i];
                packet.Write(affixNPC.mod.Name);
                packet.Write(affixNPC.GetType().FullName);
            }
        }
        public static void ReceiveMaps(BinaryReader reader)
        {
            try
            {
                int length = reader.ReadInt32();
                if (PathOfModifiers.logNetwork)
                {
                    PathOfModifiers.Log($"ReceiveMaps: {length} ");
                    PathOfModifiers.Log($"LoadedData: {affixMap.Count} ");
                }

                Dictionary<Type, int> newAffixMap = new Dictionary<Type, int>(length);
                Affix[] newAffixes = new Affix[length];

                Type type;
                Mod mod;
                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

                    if (PathOfModifiers.logNetwork)
                        PathOfModifiers.Log($"ReceiveMaps: {i} / {type.FullName} from mod {mod}");

                    newAffixes[i] = affixes[affixMap[type]];
                    newAffixMap.Add(type, i);
                }

                affixMap = newAffixMap;
                affixes = newAffixes;

                length = reader.ReadInt32();

                Dictionary<Type, int> newRarityMap = new Dictionary<Type, int>(length);
                Rarity[] newRarities = new Rarity[length];

                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

                    newRarities[i] = rarities[rarityMap[type]];
                    newRarityMap.Add(type, i);
                }

                rarityMap = newRarityMap;
                rarities = newRarities;

                length = reader.ReadInt32();

                Dictionary<Type, int> newAffixNPCMap = new Dictionary<Type, int>(length);
                AffixesNPC.Affix[] newAffixesNPC = new AffixesNPC.Affix[length];

                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

                    newAffixesNPC[i] = affixesNPC[affixNPCMap[type]];
                    newAffixNPCMap.Add(type, i);
                }

                affixNPCMap = newAffixNPCMap;
                affixesNPC = newAffixesNPC;
            }
            catch(Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }
        
        /// <summary>
        /// Returns a valid rarity for the item.
        /// </summary>
        public static Rarity RollRarity(Item item)
        {
            Tuple<Rarity, double>[] tuples = rarities
                .Where(r => r.weight > 0 && r.CanBeRolled(item))
                .Select(r => new Tuple<Rarity, double>(r, r.weight))
                .ToArray();
            WeightedRandom<Rarity> weightedRandom = new WeightedRandom<Rarity>(Main.rand, tuples);
            Rarity rarity = weightedRandom;
            return rarity;
        }
        /// <summary>
        /// Returns a valid affix for the item or null.
        /// </summary>
        public static Affix RollNewAffix(PoMItem pomItem, Item item)
        {
            if (pomItem.FreeAffixes <= 0)
            {
                return null;
            }
            Tuple<Affix, double>[] tuples = affixes
                .Where(a => a.AffixSpaceAvailable(pomItem) &&
                    a.weight > 0 &&
                    a.CanBeRolled(pomItem, item) &&
                    !pomItem.affixes.Exists(ia => ia.GetType() == a.GetType()))
                .Select(a => new Tuple<Affix, double>(a, a.weight))
                .ToArray();
            if (tuples.Length == 0)
            {
                return null;
            }
            WeightedRandom<Affix> weightedRandom = new WeightedRandom<Affix>(Main.rand, tuples);
            Affix affix = weightedRandom;
            affix = affix.Clone();
            affix.InitializeItem(pomItem);
            return affix;
        }
        public static Prefix RollNewPrefix(PoMItem pomItem, Item item)
        {
            if (pomItem.FreePrefixes <= 0)
            {
                return null;
            }
            Tuple<Affix, double>[] tuples = affixes
                .Where(a => a.AffixSpaceAvailable(pomItem) &&
                    a.weight > 0 &&
                    a.CanBeRolled(pomItem, item) &&
                    a is Prefix &&
                    !pomItem.affixes.Exists(ia => ia.GetType() == a.GetType()))
                .Select(a => new Tuple<Affix, double>(a, a.weight))
                .ToArray();
            if (tuples.Length == 0)
            {
                return null;
            }
            WeightedRandom<Affix> weightedRandom = new WeightedRandom<Affix>(Main.rand, tuples);
            Affix prefix = weightedRandom;
            prefix = prefix.Clone();
            prefix.InitializeItem(pomItem);
            return (Prefix)prefix;
        }
        public static Suffix RollNewSuffix(PoMItem pomItem, Item item)
        {
            if (pomItem.FreeSuffixes <= 0)
            {
                return null;
            }
            Tuple<Affix, double>[] tuples = affixes
                .Where(a => a.AffixSpaceAvailable(pomItem) &&
                    a.weight > 0 &&
                    a.CanBeRolled(pomItem, item) &&
                    a is Suffix &&
                    !pomItem.affixes.Exists(ia => ia.GetType() == a.GetType()))
                .Select(a => new Tuple<Affix, double>(a, a.weight))
                .ToArray();
            if (tuples.Length == 0)
            {
                return null;
            }
            WeightedRandom<Affix> weightedRandom = new WeightedRandom<Affix>(Main.rand, tuples);
            Affix suffix = weightedRandom;
            suffix = suffix.Clone();
            suffix.InitializeItem(pomItem);
            return (Suffix)suffix;
        }

        /// <summary>
        /// Returns a valid affix for the NPC or null.
        /// </summary>
        public static AffixesNPC.Affix RollNewAffix(PoMNPC pomNPC, NPC npc)
        {
            Tuple<AffixesNPC.Affix, double>[] tuples = affixesNPC
                .Where(a => 
                    a.weight > 0 &&
                    a.CanBeRolled(pomNPC, npc) &&
                    !pomNPC.affixes.Exists(ia => ia.GetType() == a.GetType()))
                .Select(a => new Tuple<AffixesNPC.Affix, double>(a, a.weight))
                .ToArray();
            if (tuples.Length == 0)
            {
                return null;
            }
            WeightedRandom<AffixesNPC.Affix> weightedRandom = new WeightedRandom<AffixesNPC.Affix>(Main.rand, tuples);
            AffixesNPC.Affix affix = weightedRandom;
            affix = affix.Clone();
            affix.InitializeNPC(pomNPC);
            return affix;
        }
        public static AffixesNPC.Prefix RollNewPrefix(PoMNPC pomNPC, NPC npc)
        {
            Tuple<AffixesNPC.Affix, double>[] tuples = affixesNPC
                .Where(a =>
                    a.weight > 0 &&
                    a.CanBeRolled(pomNPC, npc) &&
                    a is AffixesNPC.Prefix &&
                    !pomNPC.affixes.Exists(ia => ia.GetType() == a.GetType()))
                .Select(a => new Tuple<AffixesNPC.Affix, double>(a, a.weight))
                .ToArray();
            if (tuples.Length == 0)
            {
                return null;
            }
            WeightedRandom<AffixesNPC.Affix> weightedRandom = new WeightedRandom<AffixesNPC.Affix>(Main.rand, tuples);
            AffixesNPC.Affix prefix = weightedRandom;
            prefix = prefix.Clone();
            prefix.InitializeNPC(pomNPC);
            return (AffixesNPC.Prefix)prefix;
        }
        public static AffixesNPC.Suffix RollNewSuffix(PoMNPC pomNPC, NPC npc)
        {
            Tuple<AffixesNPC.Affix, double>[] tuples = affixesNPC
                .Where(a =>
                    a.weight > 0 &&
                    a.CanBeRolled(pomNPC, npc) &&
                    a is AffixesNPC.Suffix &&
                    !pomNPC.affixes.Exists(ia => ia.GetType() == a.GetType()))
                .Select(a => new Tuple<AffixesNPC.Affix, double>(a, a.weight))
                .ToArray();
            if (tuples.Length == 0)
            {
                return null;
            }
            WeightedRandom<AffixesNPC.Affix> weightedRandom = new WeightedRandom<AffixesNPC.Affix>(Main.rand, tuples);
            AffixesNPC.Affix suffix = weightedRandom;
            suffix = suffix.Clone();
            suffix.InitializeNPC(pomNPC);
            return (AffixesNPC.Suffix)suffix;
        }
    }
}
