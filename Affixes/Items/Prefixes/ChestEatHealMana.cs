using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class ChestEatHealMana : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.5f, -0.37f, 0.5),
                new TTFloat.WeightedTier(-0.37f, -0.23f, 1),
                new TTFloat.WeightedTier(-0.23f, -0.1f, 2),
                new TTFloat.WeightedTier(0.1f, 0.23f, 2),
                new TTFloat.WeightedTier(0.23f, 0.37f, 1),
                new TTFloat.WeightedTier(0.37f, 0.50f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Faint", 3),
            new WeightedTierName("Drained", 2),
            new WeightedTierName("Subdued", 0.5),
            new WeightedTierName("Bewitching", 0.5),
            new WeightedTierName("Enchanting", 2),
            new WeightedTierName("Ethereal", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsBodyArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 }% mana heal from using items";
        }

        public override void PlayerGetHealMana(Item item, Player player, Item healItem, ref float healMultiplier)
        {
            if (ItemItem.IsArmorEquipped(item, player))
            {
                healMultiplier *= Type1.GetValue();
            }
        }
    }
}
