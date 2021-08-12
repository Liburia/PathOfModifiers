using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace PathOfModifiers.Affixes.NPCs.Affixes
{
    public class MaxLife : AffixTiered<TTFloat>
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.5f, 0.75f, 0.5),
                new TTFloat.WeightedTier(0.75f, 1f, 1),
                new TTFloat.WeightedTier(1f, 1.25f, 3),
                new TTFloat.WeightedTier(1.25f, 1.5f, 1),
            },
        };

        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Fragile", 0.5),
            new WeightedTierName("Pliant", 1),
            new WeightedTierName("Healthy", 2),
            new WeightedTierName("Vigorous", 4),
        };


        public string GetTolltipText(Item item)
        {
            string increasedReduced = Type1.GetValue() >= 1 ? "increased" : "reduced";
            return $"{Type1.GetValueFormat()}% {increasedReduced} life";
        }


        public override bool CanRoll(NPCNPC pomNPC, NPC npc)
        {
            return true;
        }

        public override void SetStaticDefaults(NPCNPC pomNPC, NPC npc)
        {
            npc.lifeMax = (int)Math.Round(npc.lifeMax * Type1.GetValue());
            npc.life = (int)Math.Round(npc.life * Type1.GetValue());
        }
    }
}
