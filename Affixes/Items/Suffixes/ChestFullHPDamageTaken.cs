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
using PathOfModifiers.Projectiles;
using Terraria.DataStructures;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class ChestFullHPDamageTaken : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.2f, 0.5),
                new TTFloat.WeightedTier(0.13f, 1),
                new TTFloat.WeightedTier(0.06f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(-0.06f, 1),
                new TTFloat.WeightedTier(-0.13f, 0.5),
                new TTFloat.WeightedTier(-0.2f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Apprehension", 3),
            new WeightedTierName("of Irresolution", 2),
            new WeightedTierName("of Doubt", 0.5),
            new WeightedTierName("of Grit", 0.5),
            new WeightedTierName("of Resolution", 2),
            new WeightedTierName("of Fortitude", 3),
        };

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsBodyArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"Take { plusMinus }{ Type1.GetValueFormat() }% damage on full HP";
        }

        public override bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (AffixItemItem.IsArmorEquipped(item, player))
            {
                damageMultiplier += Type1.GetValue();
            }

            return true;
        }
    }
}