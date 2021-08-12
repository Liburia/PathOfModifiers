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
    public class WeaponDamagePerEnemyBuff : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.15f, -0.1f, 0.5),
                new TTFloat.WeightedTier(-0.1f, -0.05f, 1),
                new TTFloat.WeightedTier(-0.05f, 0, 2),
                new TTFloat.WeightedTier(0, 0.05f, 2),
                new TTFloat.WeightedTier(0.05f, 0.1f, 1),
                new TTFloat.WeightedTier(0.1f, 0.15f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Precipitating", 3),
            new WeightedTierName("Expediting", 2),
            new WeightedTierName("Urging", 0.5),
            new WeightedTierName("Impeding", 0.5),
            new WeightedTierName("Styming", 2),
            new WeightedTierName("Thwarthing", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetTolltipText()
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();

            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat }% damage per enemy buff/debuff";
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit)
        {
            float value = Type1.GetValue();
            damageMultiplier += value * PoMUtil.CountBuffs(target.buffType);
        }
        public override void ModifyHitPvp(Item item, Player player, Player target, ref float damageMultiplier, ref bool crit)
        {
            float value = Type1.GetValue();
            damageMultiplier += value * PoMUtil.CountBuffs(target.buffType);
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit, ref int hitDirection)
        {
            if (player.HeldItem == item)
            {
                float value = Type1.GetValue();
                damageMultiplier += value * PoMUtil.CountBuffs(target.buffType);
            }
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref bool crit)
        {
            if (player.HeldItem == item)
            {
                float value = Type1.GetValue();
                damageMultiplier += value * PoMUtil.CountBuffs(target.buffType);
            }
        }
    }
}
