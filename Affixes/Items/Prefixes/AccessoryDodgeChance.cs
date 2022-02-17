using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryDodgeChance : AffixTiered<TTFloat>, IPrefix
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
                new TTFloat.WeightedTier(0.0175f, 0.0200f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Evading", 0.5),
            new WeightedTierName("Dodgy", 1),
            new WeightedTierName("Eluding", 1.5),
            new WeightedTierName("Acrobatic", 2),
            new WeightedTierName("Blurred", 2.5),
            new WeightedTierName("Ghostly", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            return $"{ valueRange1 }% Dodge chance";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.dodgeChance += Type1.GetValue();
        }
    }
}
