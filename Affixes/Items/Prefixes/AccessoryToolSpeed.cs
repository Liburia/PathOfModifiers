using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryToolSpeed : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.30f, 0.20f, 0.5),
                new TTFloat.WeightedTier(0.20f, 0.10f, 1),
                new TTFloat.WeightedTier(0.10f, 0.01f, 2),
                new TTFloat.WeightedTier(-0.01f, -0.10f, 2),
                new TTFloat.WeightedTier(-0.10f, -0.20f, 1),
                new TTFloat.WeightedTier(-0.20f, -0.30f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Sluggish", 3),
            new WeightedTierName("Lazy", 2),
            new WeightedTierName("Slacking", 0.5),
            new WeightedTierName("Nimble", 0.5),
            new WeightedTierName("Agile", 2),
            new WeightedTierName("Frenzied", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '+' : '-';
            return $"{ plusMinus }{ valueRange1 }% pickaxe speed";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.pickSpeed += Type1.GetValue();
        }
    }
}
