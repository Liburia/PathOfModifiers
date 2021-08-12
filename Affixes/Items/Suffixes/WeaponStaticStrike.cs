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
using Terraria.ID;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponStaticStrike : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3f, 3),
                new TTFloat.WeightedTier(3f, 6f, 2.5),
                new TTFloat.WeightedTier(6f, 9f, 2),
                new TTFloat.WeightedTier(9f, 12f, 1.5),
                new TTFloat.WeightedTier(12f, 15f, 1),
                new TTFloat.WeightedTier(15f, 18f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 0.1f, 3),
                new TTFloat.WeightedTier(0.1f, 0.2f, 2.5),
                new TTFloat.WeightedTier(0.2f, 0.3f, 2),
                new TTFloat.WeightedTier(0.3f, 0.4f, 1.5),
                new TTFloat.WeightedTier(0.4f, 0.5f, 1),
                new TTFloat.WeightedTier(0.5f, 0.6f, 0.5),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = true,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(1f, 0.87f, 3),
                new TTFloat.WeightedTier(0.87f, 0.73f, 2.5),
                new TTFloat.WeightedTier(0.73f, 0.6f, 2),
                new TTFloat.WeightedTier(0.6f, 0.47f, 1.5),
                new TTFloat.WeightedTier(0.47f, 0.33f, 1),
                new TTFloat.WeightedTier(0.33f, 0.2f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Tantrum", 0.5),
            new WeightedTierName("of Irritation", 1),
            new WeightedTierName("of Temper", 1.5),
            new WeightedTierName("of Ire", 2),
            new WeightedTierName("of Storm", 2.5),
            new WeightedTierName("of Wrath", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetTolltipText()
        {
            return $"On hit gain a buff for {Type1.GetValueFormat(1)}s that does {Type2.GetValueFormat()}% damage every {Type3.GetValueFormat(1)}s";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Hit(item, player, damage);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            Hit(item, player, damage);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            Hit(item, player, damage);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            Hit(item, player, damage);
        }

        void Hit(Item item, Player player, int hitDamage)
        {
            if (item == player.HeldItem)
            {
                int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, 999999);

                int intervalTicks = (int)Math.Round(Type3.GetValue() * 60);
                if (intervalTicks < 1)
                {
                    intervalTicks = 1;
                }

                int durationTicks = (int)Math.Round(Type1.GetValue() * 60);
                if (durationTicks < 1)
                {
                    durationTicks = 1;
                }

                player.GetModPlayer<BuffPlayer>().AddStaticStrikeBuff(player, damage, intervalTicks, durationTicks, true);
            }
        }
    }
}