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
    public class WeaponHeal : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.005f, 2.5),
                new TTFloat.WeightedTier(0.01f, 2),
                new TTFloat.WeightedTier(0.015f, 1.5),
                new TTFloat.WeightedTier(0.02f, 1),
                new TTFloat.WeightedTier(0.025f, 0.5),
                new TTFloat.WeightedTier(0.03f, 0),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(6f, 3),
                new TTFloat.WeightedTier(5.5f, 2.5),
                new TTFloat.WeightedTier(5f, 2),
                new TTFloat.WeightedTier(4.5f, 1.5),
                new TTFloat.WeightedTier(4f, 1),
                new TTFloat.WeightedTier(3.5f, 0.5),
                new TTFloat.WeightedTier(3f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Haleness", 0.5),
            new WeightedTierName("of Well-being", 1),
            new WeightedTierName("of Health", 1.5),
            new WeightedTierName("of Constitution", 2),
            new WeightedTierName("of Vigor", 2.5),
            new WeightedTierName("of Vermilion", 3),
        };


        double lastCastTime = 0;

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"Heal {Type1.GetValueFormat()}% of max life on hit ({Type2.GetValueFormat(1)}s CD)";
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
            if (item == player.HeldItem && (PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds - lastCastTime) / 1000.0 >= Type2.GetValue())
                HealPlayer(player);
        }

        void HealPlayer(Player player)
        {
            int amount = (int)MathHelper.Clamp(player.statLifeMax2 * Type1.GetValue(), 1, 9999999);
            player.statLife += amount;
            player.HealEffect(amount, false);
            PoMEffectHelper.Heal(player, amount);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                EffectPacketHandler.CSyncHeal(player.whoAmI, amount);
            }
            lastCastTime = PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds;
        }
    }
}