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
    public class GreavesMeleeAttackSpeed : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.3f, 0.5),
                new TTFloat.WeightedTier(-0.2f, 1),
                new TTFloat.WeightedTier(-0.1f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(0.1f, 1),
                new TTFloat.WeightedTier(0.2f, 0.5),
                new TTFloat.WeightedTier(0.3f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Sluggish", 3),
            new WeightedTierName("Lazy", 2),
            new WeightedTierName("Slacking", 0.5),
            new WeightedTierName("Nimble", 0.5),
            new WeightedTierName("Agile", 2),
            new WeightedTierName("Frenzied", 3),
        };


        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsLegArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();
            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat }% melee attack speed";
        }

        public override void UpdateEquip(Item item, AffixItemPlayer player)
        {
            player.meleeSpeed += Type1.GetValue();
        }
    }
}
