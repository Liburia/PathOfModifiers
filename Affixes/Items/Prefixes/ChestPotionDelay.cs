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
    public class ChestPotionDelay : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.3f, 0.5),
                new TTFloat.WeightedTier(0.2f, 1),
                new TTFloat.WeightedTier(0.1f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(-0.1f, 1),
                new TTFloat.WeightedTier(-0.2f, 0.5),
                new TTFloat.WeightedTier(-0.3f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Quenched", 3),
            new WeightedTierName("Moist", 2),
            new WeightedTierName("Satisfied", 0.5),
            new WeightedTierName("Arid", 0.5),
            new WeightedTierName("Thirsty", 2),
            new WeightedTierName("Drought", 3),
        };


        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsBodyArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();
            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat }% potion delay";
        }

        public override void UpdateEquip(Item item, AffixItemPlayer player)
        {
            player.potionDelayTime += Type1.GetValue();
            player.restorationDelayTime += Type1.GetValue();
        }
    }
}
