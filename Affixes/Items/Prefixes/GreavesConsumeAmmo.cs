﻿using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using System.Data.Odbc;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class GreavesConsumeAmmo : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.10f, 0.13f, 3),
                new TTFloat.WeightedTier(0.13f, 0.17f, 2.5),
                new TTFloat.WeightedTier(0.17f, 0.20f, 2),
                new TTFloat.WeightedTier(0.20f, 0.23f, 1.5),
                new TTFloat.WeightedTier(0.23f, 0.27f, 1),
                new TTFloat.WeightedTier(0.27f, 0.30f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Wasteful", 0.5),
            new WeightedTierName("Stable", 1),
            new WeightedTierName("Frugal", 1.5),
            new WeightedTierName("Prudent", 2),
            new WeightedTierName("Conserving", 2.5),
            new WeightedTierName("Materialistic", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsLegArmor(item);
        }

        public override string GetTolltipText()
        {
            float valueFormat = Type1.GetValueFormat();
            return $"{ valueFormat }% chance to not consume ammo";
        }

        public override bool PlayerConsumeAmmo(Player player, Item item, Item ammo)
        {
            if (ItemItem.IsArmorEquipped(item, player))
            {
                float value = Type1.GetValue();
                return Main.rand.NextFloat(1f) > value;
            }
            return true;
        }
    }
}
