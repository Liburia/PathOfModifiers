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
using PathOfModifiers.Buffs;
using Terraria.ID;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponChainLightning : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 0.05f, 3),
                new TTFloat.WeightedTier(0.05f, 0.1f, 2.5),
                new TTFloat.WeightedTier(0.1f, 0.15f, 2),
                new TTFloat.WeightedTier(0.15f, 0.2f, 1.5),
                new TTFloat.WeightedTier(0.2f, 0.25f, 1),
                new TTFloat.WeightedTier(0.25f, 0.3f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 0.33f, 3),
                new TTFloat.WeightedTier(0.33f, 0.66f, 2.5),
                new TTFloat.WeightedTier(0.66f, 1f, 2),
                new TTFloat.WeightedTier(1f, 1.33f, 1.5),
                new TTFloat.WeightedTier(1.33f, 1.66f, 1),
                new TTFloat.WeightedTier(1.66f, 2f, 0.5),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 0.025f, 3),
                new TTFloat.WeightedTier(0.025f, 0.05f, 2.5),
                new TTFloat.WeightedTier(0.05f, 0.075f, 2),
                new TTFloat.WeightedTier(0.075f, 0.1f, 1.5),
                new TTFloat.WeightedTier(0.1f, 0.125f, 1),
                new TTFloat.WeightedTier(0.125f, 0.15f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Static", 0.5),
            new WeightedTierName("of Inertia", 1),
            new WeightedTierName("of Sparking", 1.5),
            new WeightedTierName("of Lightning", 2),
            new WeightedTierName("of Electrification", 2.5),
            new WeightedTierName("of Galvanism", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetTolltipText()
        {
            string plusMinus = Type3.GetValue() >= 0 ? "+" : "-";
            return $"{Type1.GetValueFormat()}% chance to chain lightning for {Type2.GetValueFormat()}% damage that Shocks({plusMinus}{Type3.GetValueFormat()}%)";
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
                SpawnChainLightning(player, target, hitDamage, true);
            }
        }
        void Hit(Item item, Player player, Player target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
            {
                SpawnChainLightning(player, target, hitDamage, false);
            }
        }

        void SpawnChainLightning(Player player, Entity target, int hitDamage, bool isNPC)
        {
            PlaySound(player);
            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, 999999);
            Projectile.NewProjectile(
                new PoMGlobals.ProjectileSource.PlayerSource(player),
                position: player.Center,
                velocity: new Vector2(isNPC ? 1 : 0, target.whoAmI),
                Type: ModContent.ProjectileType<ChainLightning>(),
                Damage: damage,
                KnockBack: 0,
                Owner: player.whoAmI,
                ai0: Type3.GetValue());
        }

        void PlaySound(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item72.WithVolume(0.3f).WithPitchVariance(0.3f), player.Center);
        }
    }
}