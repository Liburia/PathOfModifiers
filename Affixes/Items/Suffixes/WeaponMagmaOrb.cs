﻿using Microsoft.Xna.Framework.Graphics;
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
using Terraria.ID;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponMagmaOrb : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.05f, 2.5),
                new TTFloat.WeightedTier(0.1f, 2),
                new TTFloat.WeightedTier(0.15f, 1.5),
                new TTFloat.WeightedTier(0.2f, 1),
                new TTFloat.WeightedTier(0.25f, 0.5),
                new TTFloat.WeightedTier(0.3f, 0),
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
            TwoWay = true,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.1f, 2.5),
                new TTFloat.WeightedTier(0.2f, 2),
                new TTFloat.WeightedTier(0.3f, 1.5),
                new TTFloat.WeightedTier(0.4f, 1),
                new TTFloat.WeightedTier(0.5f, 0.5),
                new TTFloat.WeightedTier(0.6f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Embers", 0.5),
            new WeightedTierName("of Magma", 1),
            new WeightedTierName("of Flames", 1.5),
            new WeightedTierName("of Pyre", 2),
            new WeightedTierName("of Hellfire", 2.5),
            new WeightedTierName("of Inferno", 3),
        };


        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            string plusMinus = Type3.GetValue() >= 0 ? "+" : "-";

            return $"{Type1.GetValueFormat()}% chance to Magma Orb for {Type2.GetValueFormat()}% damage that leaves Burning Air({plusMinus}{Type3.GetValueFormat()}%)";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Hit(item, player, target, damage);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            Hit(item, player, target, damage);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            Hit(item, player, target, damage);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            Hit(item, player, target, damage);
        }

        void Hit(Item item, Player player, NPC target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
            {
                SpawnFireball(player, target, hitDamage);
            }
        }
        void Hit(Item item, Player player, Player target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
            {
                SpawnFireball(player, target, hitDamage);
            }
        }

        void SpawnFireball(Player player, Entity target, int hitDamage)
        {
            PlaySound(player);
            Vector2 velocity = (target.Center - player.Center).SafeNormalize(Vector2.One) * Main.rand.NextFloat(5f, 15f);
            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, 999999);
            Projectile.NewProjectile(
                position: player.Center,
                velocity: velocity,
                Type: ModContent.ProjectileType<MagmaOrb>(),
                Damage: damage,
                KnockBack: 0,
                Owner: player.whoAmI,
                ai0: hitDamage * Type3.GetValue());
        }

        void PlaySound(Player player)
        {
            Main.PlaySound(SoundID.Item69.WithVolume(1f).WithPitchVariance(0.3f), player.Center);
        }
    }
}