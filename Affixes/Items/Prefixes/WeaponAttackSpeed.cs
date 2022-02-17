using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class WeaponAttackSpeed : AffixTiered<TTFloat>, IPrefix
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
                ItemItem.IsWeapon(item) &&
                !ItemItem.IsSpear(item) &&
                !ItemItem.IsFlailOrYoyo(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            string plusMinus = Type1.GetValue() >= 0 ? "+" : "-";
            return $"{ plusMinus }{ valueRange1 }% attack speed";
        }

        public override void UseSpeedMultiplier(Item item, Player player, ref float multiplier)
        {
            multiplier += Type1.GetValue();
        }
    }
}
