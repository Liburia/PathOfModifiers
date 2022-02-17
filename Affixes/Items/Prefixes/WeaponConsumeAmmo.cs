using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class WeaponConsumeAmmo : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.10f, 0.18f, 3),
                new TTFloat.WeightedTier(0.18f, 0.27f, 2.5),
                new TTFloat.WeightedTier(0.27f, 0.35f, 2),
                new TTFloat.WeightedTier(0.35f, 0.44f, 1.5),
                new TTFloat.WeightedTier(0.44f, 0.52f, 1),
                new TTFloat.WeightedTier(0.52f, 0.60f, 0.5),
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
                ItemItem.IsWeapon(item) &&
                ItemItem.CanConsumeAmmo(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            return $"{ valueRange1 }% chance to not consume ammo";
        }

        public override bool CanConsumeAmmo(Item item, Player player)
        {
            float value = Type1.GetValue();
            return Main.rand.NextFloat(1f) > value;
        }
    }
}
