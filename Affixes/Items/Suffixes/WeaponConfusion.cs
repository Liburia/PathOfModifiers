using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using PathOfModifiers.Projectiles;
using Terraria.ID;
using PathOfModifiers.ModNet.PacketHandlers;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponConfusion : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.1f, 1.5),
                new TTFloat.WeightedTier(0.2f, 0.5),
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
                new TTFloat.WeightedTier(2f, 1.5),
                new TTFloat.WeightedTier(4f, 0.5),
                new TTFloat.WeightedTier(6f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Distraction", 0.5),
            new WeightedTierName("of Confusion", 2),
            new WeightedTierName("of Deception", 3),
        };

        double lastProcTime = 0;

        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{Type1.GetValueFormat()}% chance to confuse enemy for {Type2.GetValueFormat(1)}s";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Hit(item, player, target);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            Hit(item, player, target);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            Hit(item, player, target);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            Hit(item, player, target);
        }

        void Hit(Item item, Player player, NPC target)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(1) < Type1.GetValue())
            {
                int durationTicks = (int)Math.Round(Type2.GetValue() * 60);
                target.AddBuff(BuffID.Confused, durationTicks);
            }
        }
        void Hit(Item item, Player player, Player target)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(1) < Type1.GetValue())
            {
                int durationTicks = (int)Math.Round(Type2.GetValue() * 60);
                target.AddBuff(BuffID.Confused, durationTicks, false);
            }
        }
    }
}