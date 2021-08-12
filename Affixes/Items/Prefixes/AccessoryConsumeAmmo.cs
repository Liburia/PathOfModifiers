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

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryConsumeAmmo : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.01f, 0.025f, 3),
                new TTFloat.WeightedTier(0.025f, 0.040f, 2.5),
                new TTFloat.WeightedTier(0.040f, 0.055f, 2),
                new TTFloat.WeightedTier(0.055f, 0.070f, 1.5),
                new TTFloat.WeightedTier(0.070f, 0.085f, 1),
                new TTFloat.WeightedTier(0.085f, 0.100f, 0.5),
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
                ItemItem.IsAccessory(item);
        }

        public override string GetTolltipText()
        {
            return $"{ Type1.GetValueFormat() }% chance to not consume ammo";
        }

        public override bool PlayerConsumeAmmo(Player player, Item item, Item ammo)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
            {
                float value = Type1.GetValue();
                return Main.rand.NextFloat(1f) > value;
            }
            return true;
        }
    }
}
