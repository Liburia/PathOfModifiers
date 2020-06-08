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
    public class WeaponIcicles : AffixTiered<TTFloat, TTInt, TTFloat>, ISuffix
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
                new TTFloat.WeightedTier(0.33f, 2.5),
                new TTFloat.WeightedTier(0.66f, 2),
                new TTFloat.WeightedTier(1f, 1.5),
                new TTFloat.WeightedTier(1.33f, 1),
                new TTFloat.WeightedTier(1.66f, 0.5),
                new TTFloat.WeightedTier(2f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Bitterness", 0.5),
            new WeightedTierName("of Enmity", 1),
            new WeightedTierName("of Resentment", 1.5),
            new WeightedTierName("of Scorn", 2),
            new WeightedTierName("of Loathing", 2.5),
            new WeightedTierName("of Hatred", 3),
        };

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{Type1.GetValueFormat()}% chance to fire {Type2.GetValueFormat()} Icycles for {Type3.GetValueFormat()}% damage";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < Type1.GetValue())
                SpawnIcycles(player, target, damage);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < Type1.GetValue())
                SpawnIcycles(player, target, damage);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < Type1.GetValue())
                SpawnIcycles(player, target, damage);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(0, 1) < Type1.GetValue())
                SpawnIcycles(player, target, damage);
        }

        void SpawnIcycles(Player player, Entity target, int hitDamage)
        {
            float hitDirection = (target.Center - player.Center).ToRotation();

            int projectileNumber = Type2.GetValue();
            for (int i = 0; i < projectileNumber; i++)
            {
                Vector2 velocity = (target.Center - player.Center).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(7f, 20f);
                int damage = (int)MathHelper.Clamp(hitDamage * Type3.GetValue(), 1, 999999);
                Projectile.NewProjectile(player.Center, velocity, ModContent.ProjectileType<Icicle>(), damage, 0, player.whoAmI);
            }
        }
    }
}