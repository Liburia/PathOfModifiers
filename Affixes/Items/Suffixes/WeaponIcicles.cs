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
    public class WeaponIcicles : AffixTiered<TTFloat, TTInt, TTFloat>, ISuffix
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
        public override TTInt Type2 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = false,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(1, 2, 3),
                new TTInt.WeightedTier(2, 3, 2.5),
                new TTInt.WeightedTier(3, 4, 2),
                new TTInt.WeightedTier(4, 5, 1.5),
                new TTInt.WeightedTier(5, 6, 1),
                new TTInt.WeightedTier(6, 7, 0.5),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
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
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Bitterness", 0.5),
            new WeightedTierName("of Enmity", 1),
            new WeightedTierName("of Resentment", 1.5),
            new WeightedTierName("of Scorn", 2),
            new WeightedTierName("of Loathing", 2.5),
            new WeightedTierName("of Hatred", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(), Type2.GetMinValueFormat(), Type2.GetMaxValueFormat(), useChatTags);
            var valueRange3 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type3.GetCurrentValueFormat(), Type3.GetMinValueFormat(), Type3.GetMaxValueFormat(), useChatTags);
            return $"{ valueRange1 }% chance to fire { valueRange2 } Icycles for { valueRange3 }% damage";
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
            PlaySound(player);

            int projectileNumber = Type2.GetValue();
            for (int i = 0; i < projectileNumber; i++)
            {
                Vector2 velocity = (target.Center - player.Center).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)) * Main.rand.NextFloat(7f, 20f);
                int damage = (int)MathHelper.Clamp(hitDamage * Type3.GetValue(), 1, 999999);
                Projectile.NewProjectile(
                new PoMGlobals.ProjectileSource.PlayerSource(player), 
                player.Center, velocity, ModContent.ProjectileType<Icicle>(), damage, 0, player.whoAmI);
            }
        }

        void PlaySound(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item101.WithVolume(1f).WithPitchVariance(0.3f), player.Center);
        }
    }
}