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
using Terraria.Utilities;

namespace PathOfModifiers.AffixesItem
{
    public static class TieredAffixHelper
    {
        #region ITieredStatFloatAffix
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
        #endregion


        #region ITieredStatIntAffix
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
        #endregion

        #region ITieredStatIntValueAffix
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
        #endregion


        #region ITieredStatFloat2IntValueAffix
        public static void SetTier(ITieredStatFloat2IntValueAffix affix, int t1, int t2, int t3, bool ignore1 = false, bool ignore2 = false, bool ignore3 = false)
        {
            if (!ignore1)
            {
                affix.Tier1 = t1;
            }
            if (!ignore2)
            {
                affix.Tier2 = t2;
            }
            if (!ignore3)
            {
                affix.Tier3 = t3;
            }
            affix.AddedTextTiered = affix.TierNames[affix.CompoundTier];
            affix.AddedTextWeightTiered = affix.Weight / (float)affix.TierWeights1[affix.CompoundTier].Item2;
        }
        public static void SetTierMultiplier(ITieredStatFloat2IntValueAffix affix, float m1, float m2, bool ignore1 = false, bool ignore2 = false)
        {
            if (!ignore1)
            {
                affix.TierMultiplier1 = m1;
                affix.Multiplier1 = affix.Tiers1[affix.Tier1] + (affix.Tiers1[affix.Tier1 + 1] - affix.Tiers1[affix.Tier1]) * m1;
            }
            if (!ignore2)
            {
                affix.TierMultiplier2 = m2;
                affix.Multiplier2 = affix.Tiers2[affix.Tier2] + (affix.Tiers2[affix.Tier2 + 1] - affix.Tiers2[affix.Tier2]) * m2;
            }
        }
        public static Affix Clone(ITieredStatFloat2IntValueAffix affix, ITieredStatFloat2IntValueAffix newAffix)
        {
            SetTier(newAffix, affix.Tier1, affix.Tier2, affix.Tier3, false, false, false);
            SetTierMultiplier(newAffix, affix.TierMultiplier1, affix.TierMultiplier2, false, false);

            return (Affix)newAffix;
        }
        public static void RollTier(ITieredStatFloat2IntValueAffix affix, bool ignore1 = false, bool ignore2 = false, bool ignore3 = false)
        {
            int wr1 = -1;
            int wr2 = -1;
            int wr3 = -1;
            if (!ignore1)
            {
                wr1 = new WeightedRandom<int>(Main.rand, affix.TierWeights1);
            }
            if (!ignore2)
            {
                wr2 = new WeightedRandom<int>(Main.rand, affix.TierWeights2);
            }
            if (!ignore3)
            {
                wr3 = new WeightedRandom<int>(Main.rand, affix.TierWeights3);
            }
            SetTier(affix, wr1, wr2, wr3, ignore1, ignore2, ignore3);
        }
        public static void RollTierMultiplier(ITieredStatFloat2IntValueAffix affix, bool ignore1 = false, bool ignore2 = false)
        {
            SetTierMultiplier(affix, Main.rand.NextFloat(0, 1), Main.rand.NextFloat(0, 1), ignore1, ignore2);
        }
        public static void RollValue(ITieredStatFloat2IntValueAffix affix, bool rollTier)
        {
            if (rollTier)
                RollTier(affix);
            RollTierMultiplier(affix);
        }
        public static void ReforgePrice(ITieredStatFloat2IntValueAffix affix, Item item, ref int price)
        {
            price += (int)Math.Round(item.value * 0.2f * affix.CompoundTier / 4 / affix.Weight);
        }
        public static void Save(ITieredStatFloat2IntValueAffix affix, TagCompound tag, Item item)
        {
            tag.Set("tier1", affix.Tier1);
            tag.Set("tierMultiplier1", affix.TierMultiplier1);
            tag.Set("tier2", affix.Tier2);
            tag.Set("tierMultiplier2", affix.TierMultiplier2);
            tag.Set("tier3", affix.Tier3);
        }
        public static void Load(ITieredStatFloat2IntValueAffix affix, TagCompound tag, Item item)
        {
            SetTier(affix, tag.GetInt("tier1"), tag.GetInt("tier2"), tag.GetInt("tier3"));
            SetTierMultiplier(affix, tag.Get<float>("tierMultiplier1"), tag.Get<float>("tierMultiplier2"));
        }
        public static void NetSend(ITieredStatFloat2IntValueAffix affix, Item item, BinaryWriter writer)
        {
            writer.Write(affix.Tier1);
            writer.Write(affix.TierMultiplier1);
            writer.Write(affix.Tier2);
            writer.Write(affix.TierMultiplier2);
            writer.Write(affix.Tier3);
        }
        public static void NetReceive(ITieredStatFloat2IntValueAffix affix, Item item, BinaryReader reader)
        {
            int t1 = reader.ReadInt32();
            float tm1 = reader.ReadSingle();
            int t2 = reader.ReadInt32();
            float tm2 = reader.ReadSingle();
            int t3 = reader.ReadInt32();
            SetTier(affix, t1, t2, t3);
            SetTierMultiplier(affix, t1, t2);
        }
        #endregion
    }
}
