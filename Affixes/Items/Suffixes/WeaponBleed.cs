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
using PathOfModifiers.Buffs;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponBleed : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.16f, 2.5),
                new TTFloat.WeightedTier(0.33f, 2),
                new TTFloat.WeightedTier(0.5f, 1.5),
                new TTFloat.WeightedTier(0.66f, 1),
                new TTFloat.WeightedTier(0.84f, 0.5),
                new TTFloat.WeightedTier(1f, 0),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.33f, 2.5),
                new TTFloat.WeightedTier(0.66f, 2),
                new TTFloat.WeightedTier(1f, 1.5),
                new TTFloat.WeightedTier(1.33f, 1),
                new TTFloat.WeightedTier(1.66f, 0.5),
                new TTFloat.WeightedTier(2f, 0),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(2f, 2.5),
                new TTFloat.WeightedTier(4f, 2),
                new TTFloat.WeightedTier(6f, 1.5),
                new TTFloat.WeightedTier(8f, 1),
                new TTFloat.WeightedTier(10f, 0.5),
                new TTFloat.WeightedTier(12f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Wounding", 3),
            new WeightedTierName("of Harming", 2),
            new WeightedTierName("of Laceration", 0.5),
            new WeightedTierName("of Hemmorrhage", 0.5),
            new WeightedTierName("of Suffering", 2),
            new WeightedTierName("of Agony", 3),
        };

        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{Type1.GetValueFormat()}% chance to Bleed({Type2.GetValueFormat()}%) for {Type3.GetValueFormat(1)}s";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            OnHit(item, player, target, damage);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            OnHit(item, player, target, damage);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            OnHit(item, player, target, damage);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            OnHit(item, player, target, damage);
        }

        void OnHit(Item item, Player player, NPC target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
                Bleed(target, hitDamage);
        }
        void OnHit(Item item, Player player, Player target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
                Bleed(target, hitDamage);
        }

        void Bleed(NPC target, int hitDamage)
        {
            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, int.MaxValue);
            int duration = (int)MathHelper.Clamp(Type3.GetValue() * 60, 1, int.MaxValue);
            PoMNPC pomNPC = target.GetGlobalNPC<PoMNPC>();
            pomNPC.AddDoTBuff(target, ModContent.GetInstance<Bleeding>(), damage, duration);
        }
        void Bleed(Player target, int hitDamage)
        {
            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, int.MaxValue);
            int duration = (int)MathHelper.Clamp(Type3.GetValue() * 60, 1, int.MaxValue);
            PoMPlayer pomPlayer = target.GetModPlayer<PoMPlayer>();
            pomPlayer.AddDoTBuff(target, ModContent.GetInstance<Bleeding>(), damage, duration);
        }
    }
}