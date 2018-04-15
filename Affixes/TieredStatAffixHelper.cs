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
        public static void SetTier(ITieredStatFloatAffix affix, int tier)
        {
            affix.Tier = tier;
            affix.AddedTextTiered = affix.TierNames[tier];
            affix.AddedTextWeightTiered = affix.Weight / (float)affix.TierWeights[tier].Item2;
        }
        public static void SetTierMultiplier(ITieredStatFloatAffix affix, float tierMultiplier)
        {
            affix.TierMultiplier = tierMultiplier;
            affix.Multiplier = affix.Tiers[affix.Tier] + (affix.Tiers[affix.Tier + 1] - affix.Tiers[affix.Tier]) * tierMultiplier;
        }
        public static Affix Clone(ITieredStatFloatAffix affix, ITieredStatFloatAffix newAffix)
        {
            SetTier(newAffix, affix.Tier);
            SetTierMultiplier(newAffix, affix.TierMultiplier);

            return (Affix)newAffix;
        }
        public static void RollValue(ITieredStatFloatAffix affix, bool rollTier)
        {
            if (rollTier)
                RollTier(affix);
            RollTierMultiplier(affix);
        }
        public static void RollTier(ITieredStatFloatAffix affix)
        {
            WeightedRandom<int> weightedRandom = new WeightedRandom<int>(Main.rand, affix.TierWeights);
            SetTier(affix, weightedRandom);
        }
        public static void RollTierMultiplier(ITieredStatFloatAffix affix)
        {
            SetTierMultiplier(affix, Main.rand.NextFloat(0, 1));
        }
        public static void ReforgePrice(ITieredStatFloatAffix affix, Item item, ref int price)
        {
            price += (int)Math.Round(item.value * 0.2f * affix.Multiplier * 1 / affix.Weight);
        }
        public static void Save(ITieredStatFloatAffix affix, TagCompound tag, Item item)
        {
            tag.Set("tier", affix.Tier);
            tag.Set("tierMultiplier", affix.TierMultiplier);
        }
        public static void Load(ITieredStatFloatAffix affix, TagCompound tag, Item item)
        {
            SetTier(affix, tag.GetInt("tier"));
            SetTierMultiplier(affix, tag.Get<float>("tierMultiplier"));
        }
        public static void NetSend(ITieredStatFloatAffix affix, Item item, BinaryWriter writer)
        {
            writer.Write(affix.Tier);
            writer.Write(affix.TierMultiplier);
        }
        public static void NetReceive(ITieredStatFloatAffix affix, Item item, BinaryReader reader)
        {
            SetTier(affix, reader.ReadInt32());
            SetTierMultiplier(affix, reader.ReadSingle());
        }


        public static void SetTier(ITieredStatIntAffix affix, int tier)
        {
            affix.Tier = tier;
            affix.AddedTextTiered = affix.TierNames[tier];
            affix.AddedTextWeightTiered = affix.Weight / (float)affix.TierWeights[tier].Item2;
        }
        public static void SetTierMultiplier(ITieredStatIntAffix affix, float tierMultiplier)
        {
            affix.TierMultiplier = tierMultiplier;
            affix.Value = (int)Math.Round(affix.Tiers[affix.Tier] + (affix.Tiers[affix.Tier + 1] - affix.Tiers[affix.Tier]) * tierMultiplier);
        }
        public static Affix Clone(ITieredStatIntAffix affix, ITieredStatIntAffix newAffix)
        {
            SetTier(newAffix, affix.Tier);
            SetTierMultiplier(newAffix, affix.TierMultiplier);

            return (Affix)newAffix;
        }
        public static void RollValue(ITieredStatIntAffix affix, bool rollTier)
        {
            if (rollTier)
                RollTier(affix);
            RollTierMultiplier(affix);
        }
        public static void RollTier(ITieredStatIntAffix affix)
        {
            WeightedRandom<int> weightedRandom = new WeightedRandom<int>(Main.rand, affix.TierWeights);
            SetTier(affix, weightedRandom);
        }
        public static void RollTierMultiplier(ITieredStatIntAffix affix)
        {
            SetTierMultiplier(affix, Main.rand.NextFloat(0, 1));
        }
        public static void ReforgePrice(ITieredStatIntAffix affix, Item item, ref int price)
        {
            price += (int)Math.Round(item.value * 0.2f * (affix.TierMultiplier + 1) * Math.Sqrt(affix.Tier) / affix.Weight);
        }
        public static void Save(ITieredStatIntAffix affix, TagCompound tag, Item item)
        {
            tag.Set("tier", affix.Tier);
            tag.Set("tierMultiplier", affix.TierMultiplier);
        }
        public static void Load(ITieredStatIntAffix affix, TagCompound tag, Item item)
        {
            SetTier(affix, tag.GetInt("tier"));
            SetTierMultiplier(affix, tag.Get<float>("tierMultiplier"));
        }
        public static void NetSend(ITieredStatIntAffix affix, Item item, BinaryWriter writer)
        {
            writer.Write(affix.Tier);
            writer.Write(affix.TierMultiplier);
        }
        public static void NetReceive(ITieredStatIntAffix affix, Item item, BinaryReader reader)
        {
            SetTier(affix, reader.ReadInt32());
            SetTierMultiplier(affix, reader.ReadSingle());
        }

        public static void SetTier(ITieredStatIntValueAffix affix, int tier)
        {
            affix.Tier = tier;
            affix.AddedTextTiered = affix.TierNames[tier];
            affix.AddedTextWeightTiered = affix.Weight / (float)affix.TierWeights[tier].Item2;
        }
        public static Affix Clone(ITieredStatIntValueAffix affix, ITieredStatIntValueAffix newAffix)
        {
            SetTier(newAffix, affix.Tier);

            return (Affix)newAffix;
        }
        public static void RollValue(ITieredStatIntValueAffix affix, bool rollTier)
        {
            if (rollTier)
            {
                WeightedRandom<int> weightedRandom = new WeightedRandom<int>(Main.rand, affix.TierWeights);
                SetTier(affix, weightedRandom);
            }

        }
        public static void ReforgePrice(ITieredStatIntValueAffix affix, Item item, ref int price)
        {
            price += (int)Math.Round(item.value * 0.2f * Math.Sqrt(affix.Tier) / affix.Weight);
        }
        public static void Save(ITieredStatIntValueAffix affix, TagCompound tag, Item item)
        {
            tag.Set("tier", affix.Tier);
        }
        public static void Load(ITieredStatIntValueAffix affix, TagCompound tag, Item item)
        {
            SetTier(affix, tag.GetInt("tier"));
        }
        public static void NetSend(ITieredStatIntValueAffix affix, Item item, BinaryWriter writer)
        {
            writer.Write(affix.Tier);
        }
        public static void NetReceive(ITieredStatIntValueAffix affix, Item item, BinaryReader reader)
        {
            SetTier(affix, reader.ReadInt32());
        }
    }
}
