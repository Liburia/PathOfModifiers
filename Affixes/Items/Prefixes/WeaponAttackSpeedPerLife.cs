using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using Terraria.ModLoader;
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
                new TTFloat.WeightedTier(-0.50f, -0.37f, 0.5),
                new TTFloat.WeightedTier(-0.37f, -0.23f, 1),
                new TTFloat.WeightedTier(-0.23f, -0.10f, 2),
                new TTFloat.WeightedTier(0.10f, 0.23f, 2),
                new TTFloat.WeightedTier(0.23f, 0.37f, 1),
                new TTFloat.WeightedTier(0.37f, 0.50f, 0.5),
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

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item) &&
                !ItemItem.IsSpear(item) &&
                !ItemItem.IsFlailOrYoyo(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            string plusMinus1 = Type1.GetValue() >= 0 ? "+" : "-";
            string plusMinus2 = Type1.GetValue() >= 0 ? "-" : "+";
            return $"Up to { plusMinus1 }{ valueRange1 }% attack speed above 50% life and up to { plusMinus2 }{ valueRange1 }% below";
        }

        public override void UseTimeMultiplier(Item item, Player player, ref float multiplier)
        {
            float value = Type1.GetValue();
            multiplier += value * (((float)player.statLife / player.statLifeMax2) - 0.5f) * 2;
        }
    }
}
