using Microsoft.Xna.Framework.Graphics;
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
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-1f, -0.66f, 0.5),
                new TTFloat.WeightedTier(-0.66f, -0.33f, 1),
                new TTFloat.WeightedTier(-0.33f, 0, 2),
                new TTFloat.WeightedTier(0, 0.33f, 2),
                new TTFloat.WeightedTier(0.33f, 0.66f, 1),
                new TTFloat.WeightedTier(0.66f, 1f, 0.5),
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


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item) &&
                ItemItem.CanKnockback(item);
        }

        public override string GetTolltipText()
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();
            return $"Up to { (value < 0 ? '-' : '+') }{ valueFormat }% knockback above 50% life and up to { (value < 0 ? '+' : '-') }{ valueFormat }% below";
        }

        public override void GetWeaponKnockback(Item item, Player player, ref float multiplier)
        {
            float value = Type1.GetValue();
            multiplier += value * (((float)player.statLife / player.statLifeMax2) - 0.5f) * 2;
        }
    }
}
