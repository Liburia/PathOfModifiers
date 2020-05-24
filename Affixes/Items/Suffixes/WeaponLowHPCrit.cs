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

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponLowHPCrit : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight => 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.5f, 0.5),
                new TTFloat.WeightedTier(0.66f, 1.2),
                new TTFloat.WeightedTier(0.84f, 2),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(1.16f, 1),
                new TTFloat.WeightedTier(1.34f, 0.5),
                new TTFloat.WeightedTier(1.5f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Failure", 3),
            new WeightedTierName("of Mercy", 2),
            new WeightedTierName("of Hesitation", 0.5),
            new WeightedTierName("of Carnage", 0.5),
            new WeightedTierName("of Execution", 2),
            new WeightedTierName("of Extermination", 3),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            string plusMinus = Type1.GetValue() >= 1 ? "+" : "-";

            return $"Deal {plusMinus}{Type1.GetValueFormat() - 100}% damage to low HP enemies";
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit)
        {
            NPC realTarget = target.realLife >= 0 ? Main.npc[target.realLife] : target;
            if (item == player.HeldItem && (realTarget.life / (float)realTarget.lifeMax) <= PathOfModifiers.lowHPThreshold)
            {
                damageMultiplier += Type1.GetValue() - 1;
            }
        }
        public override void ModifyHitPvp(Item item, Player player, Player target, ref float damageMultiplier, ref bool crit)
        {
            if (item == player.HeldItem && (target.statLife / (float)target.statLifeMax2) <= PathOfModifiers.lowHPThreshold)
            {
                damageMultiplier += Type1.GetValue() - 1;
            }
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit, ref int hitDirection)
        {
            NPC realTarget = target.realLife >= 0 ? Main.npc[target.realLife] : target;
            if (item == player.HeldItem && (realTarget.life / (float)realTarget.lifeMax) <= PathOfModifiers.lowHPThreshold)
            {
                damageMultiplier += Type1.GetValue() - 1;
            }
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref bool crit)
        {
            if (item == player.HeldItem && (target.statLife / (float)target.statLifeMax2) <= PathOfModifiers.lowHPThreshold)
            {
                damageMultiplier += Type1.GetValue() - 1;
            }
        }
    }
}