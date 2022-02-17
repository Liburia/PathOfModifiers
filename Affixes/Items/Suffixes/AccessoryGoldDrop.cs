using Terraria;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class AccessoryGoldDrop : AffixTiered<TTFloat, TTInt>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.0050f, 0.0075f, 3),
                new TTFloat.WeightedTier(0.0075f, 0.0100f, 2.5),
                new TTFloat.WeightedTier(0.0100f, 0.0125f, 2),
                new TTFloat.WeightedTier(0.0125f, 0.0150f, 1.5),
                new TTFloat.WeightedTier(0.0150f, 0.0175f, 1),
                new TTFloat.WeightedTier(0.0175f, 0.020f, 0.5),
            },
        };
        public override TTInt Type2 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = false,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(1, 2, 3),
                new TTInt.WeightedTier(2, 3, 2.5),
                new TTInt.WeightedTier(3, 4, 2),
                new TTInt.WeightedTier(4, 5, 1.5),
                new TTInt.WeightedTier(5, 6, 1),
                new TTInt.WeightedTier(6, 7, 0.5),
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
                ItemItem.IsAccessory(item);
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