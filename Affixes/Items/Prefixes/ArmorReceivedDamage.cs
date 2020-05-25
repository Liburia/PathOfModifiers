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
using Terraria.DataStructures;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class ArmorReceivedDamage : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.9f, 0.5),
                new TTFloat.WeightedTier(0.93f, 1.2),
                new TTFloat.WeightedTier(0.97f, 2),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(1.03f, 1),
                new TTFloat.WeightedTier(1.07f, 0.5),
                new TTFloat.WeightedTier(1.1f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Vulnerable", 4),
            new WeightedTierName("Exposed", 2),
            new WeightedTierName("Unguarded", 0.5),
            new WeightedTierName("Covered", 0.5),
            new WeightedTierName("Ensconced", 2),
            new WeightedTierName("Guarded", 4),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsBodyArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{(Type1.GetValue() < 1 ? '-' : '+')}{Type1.GetValueFormat() - 100}% received damage";
        }

        public override bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (player.armor[1] == item)
            {
                damageMultiplier += Type1.GetValue() - 1;
            }
            return true;
        }
    }
}
