﻿using Terraria;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class ArmorGoldDrop : AffixTiered<TTFloat, TTInt>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.005f, 0.009f, 3),
                new TTFloat.WeightedTier(0.009f, 0.013f, 2.5),
                new TTFloat.WeightedTier(0.013f, 0.017f, 2),
                new TTFloat.WeightedTier(0.017f, 0.022f, 1.5),
                new TTFloat.WeightedTier(0.022f, 0.026f, 1),
                new TTFloat.WeightedTier(0.026f, 0.030f, 0.5),
            },
        };
        public override TTInt Type2 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = false,
            Tiers = new TTInt.WeightedTier[]
    {
                new TTInt.WeightedTier(1, 3, 3),
                new TTInt.WeightedTier(3, 5, 2.5),
                new TTInt.WeightedTier(5, 7, 2),
                new TTInt.WeightedTier(7, 9, 1.5),
                new TTInt.WeightedTier(9, 11, 1),
                new TTInt.WeightedTier(11, 13, 0.5),
    },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Gold", 0.5),
            new WeightedTierName("of Prosperity", 1),
            new WeightedTierName("of Riches", 1.5),
            new WeightedTierName("of Fortune", 2),
            new WeightedTierName("of Treasure", 2.5),
            new WeightedTierName("of Midas", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(), Type2.GetMinValueFormat(), Type2.GetMaxValueFormat(), useChatTags);
            return $"{ valueRange1 }% chance to drop { valueRange2 } gold on kill";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.Player.GetModPlayer<ItemPlayer>().goldDropChances.AddOrUpdate(this, Type1.GetValue(), Type2.GetValue());
        }
    }
}