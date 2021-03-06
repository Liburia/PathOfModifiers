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
    public class ArmorDodgeChance : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0, 3),
                new TTFloat.WeightedTier(0.025f, 2.5),
                new TTFloat.WeightedTier(0.05f, 2),
                new TTFloat.WeightedTier(0.075f, 1.5),
                new TTFloat.WeightedTier(0.1f, 1),
                new TTFloat.WeightedTier(0.125f, 0.5),
                new TTFloat.WeightedTier(0.15f, 0),
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
                new TTFloat.WeightedTier(7f, 3),
                new TTFloat.WeightedTier(6f, 2.5),
                new TTFloat.WeightedTier(5f, 2),
                new TTFloat.WeightedTier(4f, 1.5),
                new TTFloat.WeightedTier(3f, 1),
                new TTFloat.WeightedTier(2f, 0.5),
                new TTFloat.WeightedTier(1f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Evasion", 0.5),
            new WeightedTierName("of Dodging", 1),
            new WeightedTierName("of Elusion", 1.5),
            new WeightedTierName("of Acrobat", 2),
            new WeightedTierName("of Blur", 2.5),
            new WeightedTierName("of Ghost", 3),
        };

        public uint lastProcTime = 0;

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsAnyArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"Gain { Type1.GetValueFormat() }% Dodge chance for { Type2.GetValueFormat(1) }s when hit ({ Type3.GetValueFormat(1) }s CD)";
        }

        public override void PostHurt(Item item, Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            GainDodgeChance(item, player);
        }

        void GainDodgeChance(Item item, Player player)
        {
            if (AffixItemItem.IsArmorEquipped(item, player) && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type3.GetValue() * 60))
            {
                int durationTicks = (int)Math.Round((Type2.GetValue() * 60));
                player.GetModPlayer<BuffPlayer>().AddDodgeChanceBuff(player, Type1.GetValue(), durationTicks, false);
                lastProcTime = Main.GameUpdateCount;
            }
        }

        public override Affix Clone()
        {
            var affix = (ArmorDodgeChance)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}