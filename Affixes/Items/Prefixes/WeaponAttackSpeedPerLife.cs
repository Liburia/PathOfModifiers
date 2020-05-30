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
    public class WeaponAttackSpeedPerLife : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-1f, 0.5),
                new TTFloat.WeightedTier(-0.66f, 1),
                new TTFloat.WeightedTier(-0.33f, 2),
                new TTFloat.WeightedTier(0, 2),
                new TTFloat.WeightedTier(0.33f, 1),
                new TTFloat.WeightedTier(0.66f, 0.5),
                new TTFloat.WeightedTier(1f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Renowned", 3),
            new WeightedTierName("Notable", 2),
            new WeightedTierName("Inadequate", 0.5),
            new WeightedTierName("Amateur", 0.5),
            new WeightedTierName("Proficient", 2),
            new WeightedTierName("Skilled", 3),
        };

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsWeapon(item) &&
                !AffixItemItem.IsSpear(item) &&
                !AffixItemItem.IsFlailOrYoyo(item);
        }

        public override string GetTolltipText(Item item)
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();
            return $"Up to { (value < 0 ? '-' : '+') }{ valueFormat }% attack speed above 50% life and up to { (value < 0 ? '+' : '-') }{ valueFormat }% below";
        }

        public override void UseTimeMultiplier(Item item, Player player, ref float multiplier)
        {
            float value = Type1.GetValue();
            multiplier += value * (((float)player.statLife / player.statLifeMax2) - 0.5f);
        }
    }
}
