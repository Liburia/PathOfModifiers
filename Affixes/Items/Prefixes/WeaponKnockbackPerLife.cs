using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class WeaponKnockbackPerLife : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-1f, -0.7f, 0.5),
                new TTFloat.WeightedTier(-0.7f, -0.4f, 1),
                new TTFloat.WeightedTier(-0.4f, -0.1f, 2),
                new TTFloat.WeightedTier(0.1f, 0.4f, 2),
                new TTFloat.WeightedTier(0.4f, 0.7f, 1),
                new TTFloat.WeightedTier(0.7f, 1f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Cumbersome", 3),
            new WeightedTierName("Unhandy", 2),
            new WeightedTierName("Inconvenient", 0.5),
            new WeightedTierName("Clumsy", 0.5),
            new WeightedTierName("Unwieldy", 2),
            new WeightedTierName("Burdensome", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item) &&
                ItemItem.CanKnockback(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            string plusMinus1 = Type1.GetValue() >= 0 ? "+" : "-";
            string plusMinus2 = Type1.GetValue() >= 0 ? "-" : "+";
            return $"Up to { plusMinus1 }{ valueRange1 }% knockback above 50% life and up to { plusMinus2 }{ valueRange1 }% below";
        }

        public override void ModifyWeaponKnockback(Item item, Player player, ref float multiplier)
        {
            float value = Type1.GetValue();
            multiplier += value * (((float)player.statLife / player.statLifeMax2) - 0.5f) * 2;
        }
    }
}
