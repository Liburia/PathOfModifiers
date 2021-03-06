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
using PathOfModifiers.Buffs;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class GreavesMoveSpeed : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.6f, 0.5),
                new TTFloat.WeightedTier(-0.4f, 1),
                new TTFloat.WeightedTier(-0.2f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(0.2f, 1),
                new TTFloat.WeightedTier(0.4f, 0.5),
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
                new TTFloat.WeightedTier(1f, 2.5),
                new TTFloat.WeightedTier(2f, 2),
                new TTFloat.WeightedTier(3f, 1.5),
                new TTFloat.WeightedTier(4f, 1),
                new TTFloat.WeightedTier(5f, 0.5),
                new TTFloat.WeightedTier(6f, 0),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(8f, 3),
                new TTFloat.WeightedTier(7f, 2.5),
                new TTFloat.WeightedTier(6f, 2),
                new TTFloat.WeightedTier(5f, 1.5),
                new TTFloat.WeightedTier(4f, 1),
                new TTFloat.WeightedTier(3f, 0.5),
                new TTFloat.WeightedTier(2f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Spurring", 0.5),
            new WeightedTierName("of Hurry", 1),
            new WeightedTierName("of Quickening", 1.5),
            new WeightedTierName("of Haste", 2),
            new WeightedTierName("of Stimulation", 2.5),
            new WeightedTierName("of Rush", 3),
        };

        uint lastProcTime;

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsLegArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ Type1.GetValueFormat() }% move speed for { Type2.GetValueFormat(1) }s when hit ({ Type3.GetValueFormat(1) }s CD)";
        }

        public override void PostHurt(Item item, Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (AffixItemItem.IsArmorEquipped(item, player) && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type3.GetValue() * 60))
            {
                player.GetModPlayer<BuffPlayer>().AddGreavesMoveSpeedBuff(player, Type1.GetValue(), (int)Math.Round(Type2.GetValue() * 60), false);

                lastProcTime = Main.GameUpdateCount;
            }
        }

        public override Affix Clone()
        {
            var affix = (GreavesMoveSpeed)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}