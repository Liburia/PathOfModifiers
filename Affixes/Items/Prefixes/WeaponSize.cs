using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class WeaponSize : AffixTiered<TTFloat>, IPrefix
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
            new WeightedTierName("Tiny", 3),
            new WeightedTierName("Small", 2),
            new WeightedTierName("Modest", 0.5),
            new WeightedTierName("Bulky", 0.5),
            new WeightedTierName("Large", 2),
            new WeightedTierName("Massive", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item) &&
                ItemItem.CanMelee(item) &&
                (ItemItem.IsSwinging(item) || ItemItem.IsStabbing(item));
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 }% size";
        }

        public override void ModifyItemScale(Item item, Player player, ref float multiplier)
        {
            multiplier += Type1.GetValue();
        }
    }
}
