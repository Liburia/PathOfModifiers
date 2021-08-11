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
    public class WeaponMeleeDamage : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 0.8;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.5f, 0.5),
                new TTFloat.WeightedTier(-0.333f, 1),
                new TTFloat.WeightedTier(-0.166f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(0.166f, 1),
                new TTFloat.WeightedTier(0.333f, 0.5),
                new TTFloat.WeightedTier(0.5f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Broken", 3),
            new WeightedTierName("Damaged", 2),
            new WeightedTierName("Dull", 0.5),
            new WeightedTierName("Polished", 0.5),
            new WeightedTierName("Sharp", 2),
            new WeightedTierName("Flaring", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item) &&
                ItemItem.IsMelee(item);
        }

        public override string GetTolltipText()
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();

            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat }% melee damage";
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage, ref float flat)
        {
            float value = Type1.GetValue();
            damage += value;
        }
    }
}
