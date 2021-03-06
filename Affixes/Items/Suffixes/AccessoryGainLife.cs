﻿using Microsoft.Xna.Framework;
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
using Terraria.DataStructures;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class AccessoryGainLife : AffixTiered<TTInt, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(-30, 0.5),
                new TTInt.WeightedTier(-20, 1.5),
                new TTInt.WeightedTier(-10, 3),
                new TTInt.WeightedTier(1, 3),
                new TTInt.WeightedTier(11, 1.5),
                new TTInt.WeightedTier(21, 0.5),
                new TTInt.WeightedTier(31, 0),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(17f, 3),
                new TTFloat.WeightedTier(15f, 2.5),
                new TTFloat.WeightedTier(13f, 2),
                new TTFloat.WeightedTier(11f, 1.5),
                new TTFloat.WeightedTier(9f, 1),
                new TTFloat.WeightedTier(7f, 0.5),
                new TTFloat.WeightedTier(5f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Death", 0.5),
            new WeightedTierName("of Enervation", 1),
            new WeightedTierName("of Indolence", 1.5),
            new WeightedTierName("of Verve", 2),
            new WeightedTierName("of Vitality", 2.5),
            new WeightedTierName("of Life", 3),
        };

        public uint lastProcTime = 0;

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            string gainLose = Type1.GetValue() > 0 ? "Gain" : "Lose";
            return $"{ gainLose } { Type1.GetValueFormat() } life when hit ({ Type2.GetValueFormat(1) }s CD)";
        }

        public override void PostHurt(Item item, Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            Heal(item, player);
        }

        void Heal(Item item, Player player)
        {
            if (AffixItemItem.IsAccessoryEquipped(item, player) && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type2.GetValue() * 60))
            {
                lastProcTime = Main.GameUpdateCount;
                int amount = Type1.GetValue();
                if (amount > 0)
                {
                    player.statLife += amount;
                    PoMEffectHelper.Heal(player, amount);
                }
                else
                {
                    player.immune = false;
                    player.Hurt(PlayerDeathReason.ByPlayer(player.whoAmI), -amount, 0, false);
                }
            }
        }

        public override Affix Clone()
        {
            var affix = (AccessoryGainLife)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}