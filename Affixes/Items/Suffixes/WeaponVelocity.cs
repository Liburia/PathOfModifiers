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

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponVelocity : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-4f, 0.5),
                new TTFloat.WeightedTier(-2.6f, 1),
                new TTFloat.WeightedTier(-1.3f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(1.3f, 1),
                new TTFloat.WeightedTier(2.6f, 0.5),
                new TTFloat.WeightedTier(4f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Repulsion", 3),
            new WeightedTierName("of Repel", 2),
            new WeightedTierName("of Fending", 0.5),
            new WeightedTierName("of Pulling", 0.5),
            new WeightedTierName("of Attraction", 2),
            new WeightedTierName("of Magnetism", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetTolltipText()
        {
            string towardsAway = Type1.GetValue() >= 0 ? "towards" : "away from";
            return $"Gain {Type1.GetValueFormat(1)} velocity {towardsAway} target on hit";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            if (player.HeldItem == item)
            {
                GainVelocity(player, target);
            }
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            if (player.HeldItem == item)
            {
                GainVelocity(player, target);
            }
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            if (player.HeldItem == item)
            {
                GainVelocity(player, target);
            }
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            if (player.HeldItem == item)
            {
                GainVelocity(player, target);
            }
        }

        void GainVelocity(Player player, Entity target)
        {
            Vector2 addVelocity = (target.Center - player.Center).SafeNormalize(Vector2.Zero) * Type1.GetValue();
            player.velocity += addVelocity;
        }
    }
}
