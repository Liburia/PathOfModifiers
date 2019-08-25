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

namespace PathOfModifiers
{
	public static class PoMAffixController
    {
        /// <summary>
        /// Returns a valid rarity for the item.
        /// </summary>
        public static RarityItem RollRarity(Item item)
        {
            Tuple<RarityItem, double>[] tuples = PoMDataLoader.raritiesItem
                .Where(r => r.weight > 0 && r.CanBeRolled(item))
                .Select(r => new Tuple<RarityItem, double>(r, r.weight))
                .ToArray();
            WeightedRandom<RarityItem> weightedRandom = new WeightedRandom<RarityItem>(Main.rand, tuples);
            RarityItem rarity = weightedRandom;
            return rarity;
        }
        /// <summary>
        /// Returns a valid affix for the item or null.
        /// </summary>
        public static AffixesItem.Affix RollNewAffix(PoMItem pomItem, Item item)
        {
            if (pomItem.FreeAffixes <= 0)
            {
                return null;
            }
            Tuple<AffixesItem.Affix, double>[] tuples = PoMDataLoader.affixesItem
                .Where(a => a.AffixSpaceAvailable(pomItem) &&
                    a.weight > 0 &&
                    a.CanBeRolled(pomItem, item) &&
                    !pomItem.affixes.Exists(ia => ia.GetType() == a.GetType()))
                .Select(a => new Tuple<AffixesItem.Affix, double>(a, a.weight))
                .ToArray();
            if (tuples.Length == 0)
            {
                return null;
            }
            WeightedRandom<AffixesItem.Affix> weightedRandom = new WeightedRandom<AffixesItem.Affix>(Main.rand, tuples);
            AffixesItem.Affix affix = weightedRandom;
            affix = affix.Clone();
            affix.InitializeItem(pomItem);
            return affix;
        }
        public static AffixesItem.Prefix RollNewPrefix(PoMItem pomItem, Item item)
        {
            if (pomItem.FreePrefixes <= 0)
            {
                return null;
            }
            Tuple<AffixesItem.Affix, double>[] tuples = PoMDataLoader.affixesItem
                .Where(a => a.AffixSpaceAvailable(pomItem) &&
                    a.weight > 0 &&
                    a.CanBeRolled(pomItem, item) &&
                    a is AffixesItem.Prefix &&
                    !pomItem.affixes.Exists(ia => ia.GetType() == a.GetType()))
                .Select(a => new Tuple<AffixesItem.Affix, double>(a, a.weight))
                .ToArray();
            if (tuples.Length == 0)
            {
                return null;
            }
            WeightedRandom<AffixesItem.Affix> weightedRandom = new WeightedRandom<AffixesItem.Affix>(Main.rand, tuples);
            AffixesItem.Affix prefix = weightedRandom;
            prefix = prefix.Clone();
            prefix.InitializeItem(pomItem);
            return (AffixesItem.Prefix)prefix;
        }
        public static AffixesItem.Suffix RollNewSuffix(PoMItem pomItem, Item item)
        {
            if (pomItem.FreeSuffixes <= 0)
            {
                return null;
            }
            Tuple<AffixesItem.Affix, double>[] tuples = PoMDataLoader.affixesItem
                .Where(a => a.AffixSpaceAvailable(pomItem) &&
                    a.weight > 0 &&
                    a.CanBeRolled(pomItem, item) &&
                    a is AffixesItem.Suffix &&
                    !pomItem.affixes.Exists(ia => ia.GetType() == a.GetType()))
                .Select(a => new Tuple<AffixesItem.Affix, double>(a, a.weight))
                .ToArray();
            if (tuples.Length == 0)
            {
                return null;
            }
            WeightedRandom<AffixesItem.Affix> weightedRandom = new WeightedRandom<AffixesItem.Affix>(Main.rand, tuples);
            AffixesItem.Affix suffix = weightedRandom;
            suffix = suffix.Clone();
            suffix.InitializeItem(pomItem);
            return (AffixesItem.Suffix)suffix;
        }

        /// <summary>
        /// Returns a valid rarity for the NPC.
        /// </summary>
        public static RarityNPC RollRarity(NPC npc)
        {
            Tuple<RarityNPC, double>[] tuples = PoMDataLoader.raritiesNPC
                .Where(r => r.weight > 0 && r.CanBeRolled(npc))
                .Select(r => new Tuple<RarityNPC, double>(r, r.weight))
                .ToArray();
            WeightedRandom<RarityNPC> weightedRandom = new WeightedRandom<RarityNPC>(Main.rand, tuples);
            RarityNPC rarity = (RarityNPC)weightedRandom ?? new NPCNone();
            return rarity;
        }
        /// <summary>
        /// Returns a valid affix for the NPC or null.
        /// </summary>
        public static AffixesNPC.Affix RollNewAffix(PoMNPC pomNPC, NPC npc)
        {
            if (pomNPC.FreeAffixes <= 0)
            {
                return null;
            }
            Tuple<AffixesNPC.Affix, double>[] tuples = PoMDataLoader.affixesNPC
                .Where(a => a.AffixSpaceAvailable(pomNPC) &&
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
            affix.InitializeNPC(pomNPC, npc);
            return affix;
        }
        public static AffixesNPC.Prefix RollNewPrefix(PoMNPC pomNPC, NPC npc)
        {
            if (pomNPC.FreePrefixes <= 0)
            {
                return null;
            }
            Tuple<AffixesNPC.Affix, double>[] tuples = PoMDataLoader.affixesNPC
                .Where(a => a.AffixSpaceAvailable(pomNPC) &&
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
            prefix.InitializeNPC(pomNPC, npc);
            return (AffixesNPC.Prefix)prefix;
        }
        public static AffixesNPC.Suffix RollNewSuffix(PoMNPC pomNPC, NPC npc)
        {
            if (pomNPC.FreeSuffixes <= 0)
            {
                return null;
            }
            Tuple<AffixesNPC.Affix, double>[] tuples = PoMDataLoader.affixesNPC
                .Where(a => a.AffixSpaceAvailable(pomNPC) &&
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
            suffix.InitializeNPC(pomNPC, npc);
            return (AffixesNPC.Suffix)suffix;
        }
    }
}
