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
    public class WeaponMeteor : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
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
                new TTFloat.WeightedTier(0.66f, 2.5),
                new TTFloat.WeightedTier(1.33f, 2),
                new TTFloat.WeightedTier(2f, 1.5),
                new TTFloat.WeightedTier(2.66f, 1),
                new TTFloat.WeightedTier(3.33f, 0.5),
                new TTFloat.WeightedTier(4f, 0),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = true,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-1.5f, 0.5),
                new TTFloat.WeightedTier(-1f, 1),
                new TTFloat.WeightedTier(-0.5f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(0.5f, 1),
                new TTFloat.WeightedTier(1f, 0.5),
                new TTFloat.WeightedTier(1.5f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Flare", 0.5),
            new WeightedTierName("of Bursting", 1),
            new WeightedTierName("of Bombardment", 1.5),
            new WeightedTierName("of Eruption", 2),
            new WeightedTierName("of Explosion", 2.5),
            new WeightedTierName("of Phoenix", 3),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            string plusMinus = Type3.GetValue() >= 0 ? "+" : "-";

            return $"{Type1.GetValueFormat()}% chance to Meteor for {Type2.GetValueFormat()}% damage that Ignites({plusMinus}{Type3.GetValueFormat()}%) and leaves Burning Air({plusMinus}{Type3.GetValueFormat()}%)";
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
                SpawnMeteor(player, target, hitDamage);
            }
        }
        void Hit(Item item, Player player, Player target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
            {
                SpawnMeteor(player, target, hitDamage);
            }
        }

        void SpawnMeteor(Player player, Entity target, int hitDamage)
        {
            float height = 768f;
            Vector2 spawnOffset = new Vector2(0, -height).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
            Vector2 targetPosition = target.Center + Main.rand.NextVector2Circular(target.width, target.height);
            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, 999999);
            Projectile.NewProjectile(
                targetPosition + spawnOffset,
                targetPosition,
                ModContent.ProjectileType<Meteor>(),
                damage,
                0,
                player.whoAmI,
                ai0: hitDamage * Type3.GetValue());
        }
    }
}