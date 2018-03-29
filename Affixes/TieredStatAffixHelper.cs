using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using Terraria.ModLoader.IO;
using Terraria.ID;
using System.IO;
using System.Collections.Generic;
using PathOfModifiers.Affixes.Suffixes;
using PathOfModifiers.Affixes.Prefixes;
using Terraria.Utilities;

namespace PathOfModifiers.Affixes
{
    public static class TieredAffixHelper
    {
        public static void SetTier(ITieredStatAffix affix, int tier)
        {
            affix.Tier = tier;
            affix.AddedTextTiered = affix.TierNames[tier];
            affix.AddedTextWeightTiered = affix.Weight / (float)affix.TierWeights[tier].Item2;
        }
        public static void SetTierMultiplier(ITieredStatAffix affix, float tierMultiplier)
        {
            affix.TierMultiplier = tierMultiplier;
            affix.Multiplier = affix.Tiers[affix.Tier] + (affix.Tiers[affix.Tier + 1] - affix.Tiers[affix.Tier]) * tierMultiplier;
        }
        public static Affix Clone(ITieredStatAffix affix, ITieredStatAffix newAffix)
        {
            SetTier(newAffix, affix.Tier);
            SetTierMultiplier(newAffix, affix.TierMultiplier);

            return (Affix)newAffix;
        }
        public static void RollValue(ITieredStatAffix affix)
        {
            WeightedRandom<int> weightedRandom = new WeightedRandom<int>(Main.rand, affix.TierWeights);
            SetTier(affix, weightedRandom);
            SetTierMultiplier(affix, Main.rand.NextFloat(0, 1));
        }
        public static void ReforgePrice(ITieredStatAffix affix, Item item, ref int price)
        {
            price += (int)Math.Round(item.value * 0.2f * affix.Multiplier * 1 / affix.Weight);
        }
        public static void Save(ITieredStatAffix affix, TagCompound tag, Item item)
        {
            tag.Set("tier", affix.Tier);
            tag.Set("tierMultiplier", affix.TierMultiplier);
        }
        public static void Load(ITieredStatAffix affix, TagCompound tag, Item item)
        {
            SetTier(affix, tag.GetInt("tier"));
            SetTierMultiplier(affix, tag.Get<float>("tierMultiplier"));
        }
        public static void NetSend(ITieredStatAffix affix, Item item, BinaryWriter writer)
        {
            writer.Write(affix.Tier);
            writer.Write(affix.TierMultiplier);
        }
        public static void NetReceive(ITieredStatAffix affix, Item item, BinaryReader reader)
        {
            SetTier(affix, reader.ReadInt32());
            SetTierMultiplier(affix, reader.ReadSingle());
        }
    }
}
