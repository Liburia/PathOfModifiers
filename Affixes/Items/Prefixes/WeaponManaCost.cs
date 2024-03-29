﻿using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class WeaponManaCost : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.50f, 0.37f, 0.5),
                new TTFloat.WeightedTier(0.37f, 0.23f, 1),
                new TTFloat.WeightedTier(0.23f, 0.10f, 2),
                new TTFloat.WeightedTier(-0.10f, -0.23f, 2),
                new TTFloat.WeightedTier(-0.23f, -0.37f, 1),
                new TTFloat.WeightedTier(-0.37f, -0.50f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Lavish", 3),
            new WeightedTierName("Reckless", 2),
            new WeightedTierName("Careless", 0.5),
            new WeightedTierName("Compensating", 0.5),
            new WeightedTierName("Retaining", 2),
            new WeightedTierName("Preserving", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item) &&
                ItemItem.CanCostMana(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 }% mana cost";
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            float value = Type1.GetValue();
            mult += value;
        }
    }
}
