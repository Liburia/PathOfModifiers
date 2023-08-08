using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class ArmorConsumeBait : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.00f, 0.05f, 3),
                new TTFloat.WeightedTier(0.05f, 0.10f, 2.5),
                new TTFloat.WeightedTier(0.10f, 0.15f, 2),
                new TTFloat.WeightedTier(0.15f, 0.20f, 1.5),
                new TTFloat.WeightedTier(0.20f, 0.25f, 1),
                new TTFloat.WeightedTier(0.25f, 0.30f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Wasteful", 0.5),
            new WeightedTierName("Stable", 1),
            new WeightedTierName("Frugal", 1.5),
            new WeightedTierName("Prudent", 2),
            new WeightedTierName("Conserving", 2.5),
            new WeightedTierName("Materialistic", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            return $"{ valueRange1 }% chance to not consume bait";
        }

        public override bool? PlayerCanConsumeBait(Item item, Player player, Item bait)
        {
            bool? consume = null;
            if (ItemItem.IsArmorEquipped(item, player))
            {
                if (Main.rand.NextFloat() < Type1.GetValue())
                {
                    consume = false;
                }
            }
            return consume;
        }
    }
}
