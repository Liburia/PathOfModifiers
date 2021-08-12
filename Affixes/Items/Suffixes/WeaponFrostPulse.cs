﻿using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using PathOfModifiers.Projectiles;
using Terraria.ID;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponFrostPulse : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.06f, 0.10f, 3),
                new TTFloat.WeightedTier(0.10f, 0.14f, 2.5),
                new TTFloat.WeightedTier(0.14f, 0.18f, 2),
                new TTFloat.WeightedTier(0.18f, 0.22f, 1.5),
                new TTFloat.WeightedTier(0.22f, 0.26f, 1),
                new TTFloat.WeightedTier(0.26f, 0.30f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.2f, 0.5f, 3),
                new TTFloat.WeightedTier(0.5f, 0.8f, 2.5),
                new TTFloat.WeightedTier(0.8f, 1.1f, 2),
                new TTFloat.WeightedTier(1.1f, 1.4f, 1.5),
                new TTFloat.WeightedTier(1.4f, 1.7f, 1),
                new TTFloat.WeightedTier(1.7f, 2.0f, 0.5),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = true,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.03f, -0.05f, 3),
                new TTFloat.WeightedTier(-0.05f, -0.07f, 2.5),
                new TTFloat.WeightedTier(-0.07f, -0.09f, 2),
                new TTFloat.WeightedTier(-0.09f, -0.11f, 1.5),
                new TTFloat.WeightedTier(-0.11f, -0.13f, 1),
                new TTFloat.WeightedTier(-0.13f, -0.15f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Shivering", 0.5),
            new WeightedTierName("of Chill", 1),
            new WeightedTierName("of Frost", 1.5),
            new WeightedTierName("of Freezing", 2),
            new WeightedTierName("of Winter", 2.5),
            new WeightedTierName("of Arctic", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetTolltipText()
        {
            string plusMinus = Type1.GetValue() >= 0 ? "+" : "-";

            return $"{Type1.GetValueFormat()}% chance to Frost Pulse for {Type2.GetValueFormat()}% damage that Chills({plusMinus}{Type3.GetValueFormat()}%)";
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
                SpawnFrostPulse(player, target, hitDamage);
            }
        }
        void Hit(Item item, Player player, Player target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
            {
                SpawnFrostPulse(player, target, hitDamage);
            }
        }

        void SpawnFrostPulse(Player player, Entity target, int hitDamage)
        {
            PlaySound(player);
            Vector2 velocity = (target.Center - player.Center).SafeNormalize(Vector2.One) * Main.rand.NextFloat(10f, 20f);
            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, 999999);
            Projectile.NewProjectile(
                new PoMGlobals.ProjectileSource.PlayerSource(player),
                position: player.Center,
                velocity: velocity,
                Type: ModContent.ProjectileType<FrostPulse>(),
                Damage: damage,
                KnockBack: 0,
                Owner: player.whoAmI,
                ai0: Type3.GetValue());
        }

        void PlaySound(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item28.WithVolume(0.3f).WithPitchVariance(0.3f), player.Center);
        }
    }
}