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
                .Where(r => r.Weight > 0 && r.CanBeRolled(item))
                .Select(r => new Tuple<RarityItem, double>(r, r.Weight))
                .ToArray();
            WeightedRandom<RarityItem> weightedRandom = new WeightedRandom<RarityItem>(Main.rand, tuples);
            RarityItem rarity = weightedRandom;
            return rarity;
        }
        /// <summary>
        /// Returns a valid affix for the item or null.
        /// </summary>
        public static Affixes.Items.Affix RollNewAffix(PoMItem pomItem, Item item)
        {
            if (pomItem.FreeAffixes <= 0)
            {
                return null;
            }
            Tuple<Affixes.Items.Affix, double>[] tuples = PoMDataLoader.affixesItem
                .Where(a => a.AffixSpaceAvailable(pomItem) &&
                    a.Weight > 0 &&
                    a.CanBeRolled(pomItem, item) &&
                    !pomItem.affixes.Exists(ia => ia.GetType() == a.GetType()))
                .Select(a => new Tuple<Affixes.Items.Affix, double>(a, a.Weight))
                .ToArray();
            if (tuples.Length == 0)
            {
                return null;
            }
            WeightedRandom<Affixes.Items.Affix> weightedRandom = new WeightedRandom<Affixes.Items.Affix>(Main.rand, tuples);
            Affixes.Items.Affix affix = weightedRandom;
            affix = affix.Clone();
            affix.InitializeItem(pomItem);
            return affix;
        }
        public static Affixes.Items.Affix RollNewPrefix(PoMItem pomItem, Item item)
        {
            if (pomItem.FreePrefixes <= 0)
            {
                return null;
            }
            Tuple<Affixes.Items.Affix, double>[] tuples = PoMDataLoader.affixesItem
                .Where(a => a.AffixSpaceAvailable(pomItem) &&
                    a.Weight > 0 &&
                    a.CanBeRolled(pomItem, item) &&
                    a is Affixes.IPrefix &&
                    !pomItem.affixes.Exists(ia => ia.GetType() == a.GetType()))
                .Select(a => new Tuple<Affixes.Items.Affix, double>(a, a.Weight))
                .ToArray();
            if (tuples.Length == 0)
            {
                return null;
            }
            WeightedRandom<Affixes.Items.Affix> weightedRandom = new WeightedRandom<Affixes.Items.Affix>(Main.rand, tuples);
            Affixes.Items.Affix prefix = weightedRandom;
            prefix = prefix.Clone();
            prefix.InitializeItem(pomItem);
            return prefix;
        }
        public static Affixes.Items.Affix RollNewSuffix(PoMItem pomItem, Item item)
        {
            if (pomItem.FreeSuffixes <= 0)
            {
                return null;
            }
            Tuple<Affixes.Items.Affix, double>[] tuples = PoMDataLoader.affixesItem
                .Where(a => a.AffixSpaceAvailable(pomItem) &&
                    a.Weight > 0 &&
                    a.CanBeRolled(pomItem, item) &&
                    a is Affixes.ISuffix &&
                    !pomItem.affixes.Exists(ia => ia.GetType() == a.GetType()))
                .Select(a => new Tuple<Affixes.Items.Affix, double>(a, a.Weight))
                .ToArray();
            if (tuples.Length == 0)
            {
                return null;
            }
            WeightedRandom<Affixes.Items.Affix> weightedRandom = new WeightedRandom<Affixes.Items.Affix>(Main.rand, tuples);
            Affixes.Items.Affix suffix = weightedRandom;
            suffix = suffix.Clone();
            suffix.InitializeItem(pomItem);
            return suffix;
        }

        /// <summary>
        /// Returns a valid rarity for the NPC.
        /// </summary>
        public static RarityNPC RollRarity(NPC npc)
        {
            Tuple<RarityNPC, double>[] tuples = PoMDataLoader.raritiesNPC
                .Where(r => r.Weight > 0 && r.CanBeRolled(npc))
                .Select(r => new Tuple<RarityNPC, double>(r, r.Weight))
                .ToArray();
            WeightedRandom<RarityNPC> weightedRandom = new WeightedRandom<RarityNPC>(Main.rand, tuples);
            RarityNPC rarity = (RarityNPC)weightedRandom ?? new NPCNone();
            return rarity;
        }
        /// <summary>
        /// Returns a valid affix for the NPC or null.
        /// </summary>
        public static Affixes.NPCs.Affix RollNewAffix(PoMNPC pomNPC, NPC npc)
        {
            if (pomNPC.FreeAffixes <= 0)
            {
                return null;
            }
            Tuple<Affixes.NPCs.Affix, double>[] tuples = PoMDataLoader.affixesNPC
                .Where(a => a.AffixSpaceAvailable(pomNPC) &&
                    a.Weight > 0 &&
                    a.CanBeRolled(pomNPC, npc) &&
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
    }
}
