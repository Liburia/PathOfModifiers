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
    public class WeaponLifeOrbs : AffixTiered<TTFloat, TTInt, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
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
        public override TTInt Type2 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = false,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(1, 3),
                new TTInt.WeightedTier(2, 2.5),
                new TTInt.WeightedTier(3, 2),
                new TTInt.WeightedTier(4, 1.5),
                new TTInt.WeightedTier(5, 1),
                new TTInt.WeightedTier(6, 0.5),
                new TTInt.WeightedTier(7, 0),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.0016f, 2.5),
                new TTFloat.WeightedTier(0.0033f, 2),
                new TTFloat.WeightedTier(0.005f, 1.5),
                new TTFloat.WeightedTier(0.0066f, 1),
                new TTFloat.WeightedTier(0.0083f, 0.5),
                new TTFloat.WeightedTier(0.001f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Blood", 0.5),
            new WeightedTierName("of Bloodthirst", 1),
            new WeightedTierName("of Crimson", 1.5),
            new WeightedTierName("of Scarlet", 2),
            new WeightedTierName("of Sanguine", 2.5),
            new WeightedTierName("of Vampirism", 3),
        };


        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{Type1.GetValueFormat()}% chance to release {Type2.GetValueFormat()} life orbs on hit that heal {Type3.GetValueFormat()}% of damage dealt";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < Type1.GetValue())
                SpawnLifeOrbs(player, target, damage);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < Type1.GetValue())
                SpawnLifeOrbs(player, target, damage);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < Type1.GetValue())
                SpawnLifeOrbs(player, target, damage);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < Type1.GetValue())
                SpawnLifeOrbs(player, target, damage);
        }

        void SpawnLifeOrbs(Player player, Entity target, int damage)
        {
            int projectileNumber = Type2.GetValue();
            for (int i = 0; i < projectileNumber; i++)
            {
                Vector2 direction = Main.rand.NextVector2Unit();
                Vector2 velocity = direction * Main.rand.NextFloat(5, 10);
                Vector2 projTarget = player.Center + Main.rand.NextVector2Circular(player.width * 1.5f, player.height * 1.5f);
                int heal = (int)MathHelper.Clamp(damage * Type3.GetValue(), 1, 999999);
                Projectile.NewProjectile(target.Center, velocity, ModContent.ProjectileType<LifeOrb>(), heal, 0, player.whoAmI, projTarget.X, projTarget.Y);
            }
        }
    }
}