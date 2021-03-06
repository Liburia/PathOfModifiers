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
    public class AccessoryManaRestore : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.04f, 3),
                new TTFloat.WeightedTier(0.05f, 2.5),
                new TTFloat.WeightedTier(0.06f, 2),
                new TTFloat.WeightedTier(0.07f, 1.5),
                new TTFloat.WeightedTier(0.08f, 1),
                new TTFloat.WeightedTier(0.09f, 0.5),
                new TTFloat.WeightedTier(0.1f, 0),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.1f, 3),
                new TTFloat.WeightedTier(0.084f, 2.5),
                new TTFloat.WeightedTier(0.066f, 2),
                new TTFloat.WeightedTier(0.05f, 1.5),
                new TTFloat.WeightedTier(0.033f, 1),
                new TTFloat.WeightedTier(0.016f, 0.5),
                new TTFloat.WeightedTier(0f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Intuition", 0.5),
            new WeightedTierName("of Insight", 1),
            new WeightedTierName("of Acumen", 1.5),
            new WeightedTierName("of Ascendance", 2),
            new WeightedTierName("of Transcendence", 2.5),
            new WeightedTierName("of Divinity", 3),
        };

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"Restore { Type1.GetValueFormat() }% mana to increase damage taken by { Type2.GetValueFormat() }%";
        }

        public override bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (AffixItemItem.IsAccessoryEquipped(item, player))
            {
                int manaAmount = (int)Math.Round(player.statManaMax2 * Type1.GetValue());
                player.statMana += manaAmount;
                PoMEffectHelper.HealMana(player, manaAmount);
                damageMultiplier += Type2.GetValue();
            }

            return true;
        }
    }
}