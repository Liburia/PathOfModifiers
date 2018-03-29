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

            int affixIndex = 0;
            int rarityIndex = 0;
            Affix affix;
            Rarity rarity;
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
                        PathOfModifiers.Log($"Path of Modifiers: Added affix {t.FullName} with index {affixIndex} from mod {mod.Name}");
                    }
                    else if(t.IsSubclassOf(typeof(Rarity)) && t != typeof(Rarity))
                    {
                        rarity = (Rarity)Activator.CreateInstance(t);
                        rarity.mod = mod;
                        rarityList.Add(rarity);
                        rarityMap.Add(t, rarityIndex);
                        rarityIndex++;
                        PathOfModifiers.Log($"Path of Modifiers: Added rarity {t.FullName} with index {rarityIndex} from mod {mod.Name}");
                    }
                }
            }
            affixes = affixList.ToArray();
            rarities = rarityList.ToArray();
        }
        public static void Unload()
        {
            mods = new List<Mod>();

            affixMap = new Dictionary<Type, int>();
            affixes = new Affix[0];

            rarityMap = new Dictionary<Type, int>();
            rarities = new Rarity[0];
        }

        public static void SendMaps(ModPacket packet)
        {
            packet.Write(affixes.Length);
            PathOfModifiers.Log($"SendMaps: {affixes.Length} ");
            Affix affix;
            for (int i = 0; i < affixes.Length; i++)
            {
                affix = affixes[i];
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
        }
        public static void ReceiveMaps(BinaryReader reader)
        {
            try
            {
                int length = reader.ReadInt32();
                PathOfModifiers.Log($"ReceiveMaps: {length} ");
                PathOfModifiers.Log($"LoadedData: {affixMap.Count} ");

                Dictionary<Type, int> newAffixMap = new Dictionary<Type, int>(length);
                Affix[] newAffixes = new Affix[length];

                Type type;
                Mod mod;
                for (int i = 0; i < length; i++)
                {
                    mod = ModLoader.GetMod(reader.ReadString());
                    type = mod.Code.GetType(reader.ReadString(), true);

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
            }
            catch(Exception e)
            {
                ErrorLogger.Log(e.ToString());
            }
        }
        

        /// <summary>
        /// Returns a valid rarity for the item.
        /// </summary>
        public static Rarity RollRarity(PoMItem item)
        {
            Tuple<Rarity, double>[] tuples = rarities.Select(r => new Tuple<Rarity, double>(r, r.weight)).ToArray();
            WeightedRandom<Rarity> weightedRandom = new WeightedRandom<Rarity>(Main.rand, tuples);
            Rarity rarity = weightedRandom;
            rarity = rarity.Clone();
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
	}
}
