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
    public class ArmorThrowingDamage : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight => 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.9f, 0.5),
                new TTFloat.WeightedTier(0.93f, 1.2),
                new TTFloat.WeightedTier(0.97f, 2),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(1.03f, 1),
                new TTFloat.WeightedTier(1.07f, 0.5),
                new TTFloat.WeightedTier(1.1f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Awkward", 4),
            new WeightedTierName("Slipping", 2),
            new WeightedTierName("Inaccurate", 0.5),
            new WeightedTierName("Flinging", 0.5),
            new WeightedTierName("Darting", 2),
            new WeightedTierName("Assassinating", 4),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsHeadArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{(Type1.GetValue() < 1 ? '-' : '+')}{Type1.GetValueFormat() - 100}% throwing damage";
        }

        public override void UpdateEquip(Item item, PoMPlayer player)
        {
            player.throwingDamage += Type1.GetValue() - 1;
        }
    }
}