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

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryCritDamage : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.15f, 0.5),
                new TTFloat.WeightedTier(-0.1f, 1),
                new TTFloat.WeightedTier(-0.05f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(0.05f, 1),
                new TTFloat.WeightedTier(0.1f, 0.5),
                new TTFloat.WeightedTier(0.15f, 0),
            },
        };

        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Gentle", 3),
            new WeightedTierName("Calm", 2),
            new WeightedTierName("Tame", 0.5),
            new WeightedTierName("Fierce", 0.5),
            new WeightedTierName("Ferocious", 2),
            new WeightedTierName("Savage", 3),
        };


        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();
            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat }% crit damage";
        }

        public override void PlayerModifyHitNPC(Item affixItem, Player player, Item item, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit)
        {
            if (crit && AffixItemItem.IsAccessoryEquipped(item, player))
                damageMultiplier += Type1.GetValue();
        }
        public override void PlayerModifyHitPvp(Item affixItem, Player player, Item item, Player target, ref float damageMultiplier, ref bool crit)
        {
            if (crit && AffixItemItem.IsAccessoryEquipped(item, player))
                damageMultiplier += Type1.GetValue();
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit, ref int hitDirection)
        {
            if (crit && AffixItemItem.IsAccessoryEquipped(item, player))
                damageMultiplier += Type1.GetValue();
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref bool crit)
        {
            if (crit && AffixItemItem.IsAccessoryEquipped(item, player))
                damageMultiplier += Type1.GetValue();
        }
    }
}
