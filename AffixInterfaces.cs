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

namespace PathOfModifiers
{
    public interface ITieredStatFloatAffix
    {
        float Weight { get; }
        float[] Tiers { get; }
        Tuple<int, double>[] TierWeights { get; }
        string[] TierNames { get; }
        int MaxTier { get; }

        int TierText { get; }

        int Tier { get; set; }
        string AddedTextTiered { get; set; }
        float AddedTextWeightTiered { get; set; }

        float TierMultiplier { get; set; }
        float Multiplier { get; set; }

        string GetTolltipText(Item item);
    }
    public interface ITieredStatFloat2IntValueAffix
    {
        float Weight { get; }

        string[] TierNames { get; }
        int CompoundTierText { get; }
        string AddedTextTiered { get; set; }
        float AddedTextWeightTiered { get; set; }
        int CompoundTier { get; }
        int MaxCompoundTier { get; }

        float[] Tiers1 { get; }
        Tuple<int, double>[] TierWeights1 { get; }
        int MaxTier1 { get; }

        float[] Tiers2 { get; }
        Tuple<int, double>[] TierWeights2 { get; }
        int MaxTier2 { get; }

        int[] Tiers3 { get; }
        Tuple<int, double>[] TierWeights3 { get; }
        int MaxTier3 { get; }

        int Tier1 { get; set; }
        float TierMultiplier1 { get; set; }
        float Multiplier1 { get; set; }
        int TierText1 { get; }

        int Tier2 { get; set; }
        float TierMultiplier2 { get; set; }
        float Multiplier2 { get; set; }
        int TierText2 { get; }

        int Tier3 { get; set; }
        int TierText3 { get; }

        string GetTolltipText(Item item);
    }
    public interface ITieredStatIntValueAffix
    {
        float Weight { get; }
        int[] Tiers { get; }
        Tuple<int, double>[] TierWeights { get; }
        string[] TierNames { get; }
        int MaxTier { get; }

        int TierText { get; }

        int Tier { get; set; }
        string AddedTextTiered { get; set; }
        float AddedTextWeightTiered { get; set; }

        string GetTolltipText(Item item);
    }
    public interface ITieredStatIntAffix
    {
        float Weight { get; }
        float[] Tiers { get; }
        Tuple<int, double>[] TierWeights { get; }
        string[] TierNames { get; }
        int MaxTier { get; }

        int TierText { get; }

        int Tier { get; set; }
        string AddedTextTiered { get; set; }
        float AddedTextWeightTiered { get; set; }

        float TierMultiplier { get; set; }
        int Value { get; set; }

        string GetTolltipText(Item item);
    }
}
