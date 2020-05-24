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
        public override double Weight => 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.85f, 0.5),
                new TTFloat.WeightedTier(0.92f, 3),
                new TTFloat.WeightedTier(1f, 3),
                new TTFloat.WeightedTier(1.08f, 0.5),
                new TTFloat.WeightedTier(1.15f, 0),
            },
        };

        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Gentle", 4),
            new WeightedTierName("Calm", 1.5),
            new WeightedTierName("Fierce", 1.5),
            new WeightedTierName("Savage", 4),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{(Type1.GetValue() < 1 ? '-' : '+')}{Type1.GetValueFormat() - 100}% crit damage";
        }

        public override void PlayerModifyHitNPC(Item affixItem, Player player, Item item, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit)
        {
            if (crit && PoMItem.IsAccessoryEquipped(item, player))
                damageMultiplier += Type1.GetValue() - 1;
        }
        public override void PlayerModifyHitPvp(Item affixItem, Player player, Item item, Player target, ref float damageMultiplier, ref bool crit)
        {
            if (crit && PoMItem.IsAccessoryEquipped(item, player))
                damageMultiplier += Type1.GetValue() - 1;
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit, ref int hitDirection)
        {
            if (crit && PoMItem.IsAccessoryEquipped(item, player))
                damageMultiplier += Type1.GetValue() - 1;
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref bool crit)
        {
            if (crit && PoMItem.IsAccessoryEquipped(item, player))
                damageMultiplier += Type1.GetValue() - 1;
        }
    }
}
