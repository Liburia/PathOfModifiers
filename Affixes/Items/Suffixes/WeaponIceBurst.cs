using Microsoft.Xna.Framework.Graphics;
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
    public class WeaponIceBurst : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
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
            new WeightedTierName("of Slush", 0.5),
            new WeightedTierName("of Hail", 1),
            new WeightedTierName("of Snow", 1.5),
            new WeightedTierName("of Ice", 2),
            new WeightedTierName("of Glacier", 2.5),
            new WeightedTierName("of Permafrost", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetTolltipText()
        {
            string plusMinus = Type3.GetValue() >= 0 ? "+" : "-";

            return $"{Type1.GetValueFormat()}% chance to Ice Burst for {Type2.GetValueFormat()}% damage that leaves Chilled Air({ plusMinus }{Type3.GetValueFormat()}%)";
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
                SpawnBurst(player, target, hitDamage);
            }
        }
        void Hit(Item item, Player player, Player target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
            {
                SpawnBurst(player, target, hitDamage);
            }
        }

        void SpawnBurst(Player player, Entity target, int hitDamage)
        {
            PlaySound(player);

            int spikeCount = 5;
            float speed = 20;
            float halfArc = 0.7f;
            float arc = halfArc * 2;
            float angleIncrement = arc / (spikeCount - 1);
            float angle = (target.Center - player.Center).ToRotation() - halfArc;
            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, 999999);

            int spawnerBurst = spikeCount / 2;
            for (int i = 0; i < spikeCount; i++)
            {
                Vector2 velocity = angle.ToRotationVector2() * speed;
                Projectile.NewProjectile(
                new PoMGlobals.ProjectileSource.PlayerSource(player),
                    position: player.Center,
                    velocity: velocity,
                    Type: ModContent.ProjectileType<IceBurst>(),
                    Damage: damage,
                    KnockBack: 0,
                    Owner: player.whoAmI,
                    ai0: Type3.GetValue(),
                    ai1: i == spawnerBurst ? 1f : 0f);
                angle += angleIncrement;
            }
        }

        void PlaySound(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item66.WithVolume(0.5f).WithPitchVariance(0.3f), player.Center);
        }
    }
}