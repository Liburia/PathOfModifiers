using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryLifeRegen : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.1f, -0.07f, 0.5),
                new TTFloat.WeightedTier(-0.07f, -0.04f, 1),
                new TTFloat.WeightedTier(-0.04f, 0.01f, 2),
                new TTFloat.WeightedTier(0.01f, 0.04f, 2),
                new TTFloat.WeightedTier(0.04f, 0.07f, 1),
                new TTFloat.WeightedTier(0.07f, 0.1f, 0.5),
            },
        };

        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Fragile", 3),
            new WeightedTierName("Feeble", 2),
            new WeightedTierName("Weak", 0.5),
            new WeightedTierName("Tough", 0.5),
            new WeightedTierName("Healthy", 2),
            new WeightedTierName("Vigorous", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 }% life regen";
        }

        public override void NaturalLifeRegen(Item item, Player player, ref float regenMultiplier)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
            {
                regenMultiplier += Type1.GetValue();
            }
        }
    }
}
