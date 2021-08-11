using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class ChestReceivedDamage : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.1f, 0.5),
                new TTFloat.WeightedTier(0.07f, 1),
                new TTFloat.WeightedTier(0.03f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(-0.03f, 1),
                new TTFloat.WeightedTier(-0.07f, 0.5),
                new TTFloat.WeightedTier(-0.1f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Vulnerable", 3),
            new WeightedTierName("Exposed", 2),
            new WeightedTierName("Unguarded", 0.5),
            new WeightedTierName("Covered", 0.5),
            new WeightedTierName("Ensconced", 2),
            new WeightedTierName("Guarded", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsBodyArmor(item);
        }

        public override string GetTolltipText()
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();
            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat }% received damage";
        }

        public override bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (ItemItem.IsArmorEquipped(item, player))
            {
                damageMultiplier += Type1.GetValue();
            }
            return true;
        }
    }
}
