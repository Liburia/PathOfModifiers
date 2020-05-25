﻿using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class WeaponKnockbackPerLife : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight => 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.99f, 0.5),
                new TTFloat.WeightedTier(0.993f, 1),
                new TTFloat.WeightedTier(0.997f, 2),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(1.003f, 1),
                new TTFloat.WeightedTier(1.007f, 0.5),
                new TTFloat.WeightedTier(1.01f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Cumbersome", 3),
            new WeightedTierName("Unhandy", 2),
            new WeightedTierName("Inconvenient", 0.5),
            new WeightedTierName("Clumsy", 0.5),
            new WeightedTierName("Unwieldy", 2),
            new WeightedTierName("Burdensome", 3),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item) &&
                PoMItem.CanKnockback(item);
        }

        public override void GetWeaponKnockback(Item item, Player player, ref float multiplier)
        {
            multiplier += (Type1.GetValue() - 1) * ((((float)player.statLife / player.statLifeMax2) * 100) - 50);
        }

        public override string GetTolltipText(Item item)
        {
            return $"Up to {(Type1.GetValue() < 1 ? '-' : '+')}{Type1.GetValueFormat() - 100}% knockback above 50% life and up to {(Type1.GetValue() < 1 ? '+' : '-')}{Type1.GetValueFormat() - 100}% below";
        }
    }
}