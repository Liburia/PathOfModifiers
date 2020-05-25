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
using Terraria.ID;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponLiquid : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
    {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(30f, 1.5),
                new TTFloat.WeightedTier(60f, 0.5),
                new TTFloat.WeightedTier(90f, 0),
    },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of River", 0),
            new WeightedTierName("of Sea", 1.5),
            new WeightedTierName("of Ocean", 3),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"Gain liquid buffs on hit for {Type1.GetValueFormat(1)}s";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Hit(item, player);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            Hit(item, player);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            Hit(item, player);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            Hit(item, player);
        }

        void Hit(Item item, Player player)
        {
            if (player.HeldItem == item)
            {
                int durationTicks = (int)Math.Round(Type1.GetValue() * 60);
                player.AddBuff(BuffID.Flipper, durationTicks);
                player.AddBuff(BuffID.Gills, durationTicks);
                player.AddBuff(BuffID.WaterWalking, durationTicks);
            }
        }
    }
}
