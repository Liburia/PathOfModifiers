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
    public class WeaponMoveSpeed : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
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
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.5f, 2.5),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(1.5f, 1.5),
                new TTFloat.WeightedTier(2f, 1),
                new TTFloat.WeightedTier(2.5f, 0.5),
                new TTFloat.WeightedTier(3f, 0),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(6f, 3),
                new TTFloat.WeightedTier(5f, 2.5),
                new TTFloat.WeightedTier(4f, 2),
                new TTFloat.WeightedTier(3f, 1.5),
                new TTFloat.WeightedTier(2f, 1),
                new TTFloat.WeightedTier(1f, 0.5),
                new TTFloat.WeightedTier(0f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Spurring", 0.5),
            new WeightedTierName("of Hurrying", 1),
            new WeightedTierName("of Quickening", 1.5),
            new WeightedTierName("of Hastening", 2),
            new WeightedTierName("of Stimulation", 2.5),
            new WeightedTierName("of Rushing", 3),
        };


        double lastProc = 0;

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            string plusMinus = Type3.GetValue() >= 1 ? "+" : "-";

            return $"Gain {plusMinus}{Type1.GetValueFormat()}% move speed on hit for {Type2.GetValueFormat(1)}s ({Type3.GetValueFormat(1)}s CD)";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            OnHit(item, player);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            OnHit(item, player);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            OnHit(item, player);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            OnHit(item, player);
        }

        void OnHit(Item item, Player player)
        {
            if (item == player.HeldItem && (PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds - lastProc) / 1000.0 >= Type3.GetValue())
                GainMoveSpeed(player);
        }

        void GainMoveSpeed(Player player)
        {
            int duration = (int)MathHelper.Clamp(Type2.GetValue() * 60, 1, 9999999);
            player.GetModPlayer<BuffPlayer>().AddMoveSpeedBuff(player, Type1.GetValue(), duration);
            lastProc = PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds;
        }
    }
}