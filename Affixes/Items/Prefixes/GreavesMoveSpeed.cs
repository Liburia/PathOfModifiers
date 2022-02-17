using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class GreavesMoveSpeed : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.30f, -0.22f, 0.5),
                new TTFloat.WeightedTier(-0.22f, -0.13f, 1),
                new TTFloat.WeightedTier(-0.13f, -0.05f, 2),
                new TTFloat.WeightedTier(0.05f, 0.13f, 2),
                new TTFloat.WeightedTier(0.13f, 0.22f, 1),
                new TTFloat.WeightedTier(0.22f, 0.30f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Lethargic", 3),
            new WeightedTierName("Slow", 2),
            new WeightedTierName("Leisurly", 0.5),
            new WeightedTierName("Quick", 0.5),
            new WeightedTierName("Swift", 2),
            new WeightedTierName("Fleeting", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsLegArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 }% movement speed";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.moveSpeed += Type1.GetValue();
        }
    }
}
