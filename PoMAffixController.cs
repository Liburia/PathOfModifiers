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
        /// <summary>
        /// Returns a valid rarity for the item.
        /// </summary>
        public static Rarity RollRarity(Item item)
        {
            Tuple<Rarity, double>[] tuples = PoMDataLoader.rarities
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
            Tuple<Affix, double>[] tuples = PoMDataLoader.affixes
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
            Tuple<Affix, double>[] tuples = PoMDataLoader.affixes
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
            Tuple<Affix, double>[] tuples = PoMDataLoader.affixes
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
    }
}
