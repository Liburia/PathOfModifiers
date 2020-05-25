﻿using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryMeleeDamage : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight => 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.9f, 0.5),
                new TTFloat.WeightedTier(0.95f, 3),
                new TTFloat.WeightedTier(1f, 3),
                new TTFloat.WeightedTier(1.05f, 0.5),
                new TTFloat.WeightedTier(1.1f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Broken", 4),
            new WeightedTierName("Dull", 1.5),
            new WeightedTierName("Polished", 1.5),
            new WeightedTierName("Flaring", 4),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{(Type1.GetValue() < 1 ? '-' : '+')}{Type1.GetValueFormat() - 100}% melee damage";
        }

        public override void UpdateEquip(Item item, PoMPlayer player)
        {
            player.meleeDamage += Type1.GetValue() - 1;
        }
    }
}