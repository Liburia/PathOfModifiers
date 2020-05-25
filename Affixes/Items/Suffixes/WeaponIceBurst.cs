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
    public class WeaponIceBurst : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
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
                new TTFloat.WeightedTier(0.9f, 0.5),
                new TTFloat.WeightedTier(0.93f, 1),
                new TTFloat.WeightedTier(0.97f, 2),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(1.03f, 1),
                new TTFloat.WeightedTier(1.07f, 0.5),
                new TTFloat.WeightedTier(1.1f, 0),
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


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            string plusMinus = Type3.GetValue() >= 1 ? "+" : "-";

            return $"{Type1.GetValueFormat()}% chance to Ice Burst for {Type2.GetValueFormat()}% damage that leaves Chilled Air({Type3.GetValueFormat() - 100}%)";
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
    }
}