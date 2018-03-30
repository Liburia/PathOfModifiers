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

namespace PathOfModifiers.Affixes
{
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
    }
}
