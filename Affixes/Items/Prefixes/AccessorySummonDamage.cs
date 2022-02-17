using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessorySummonDamage : AffixTiered<TTFloat>, IPrefix
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
            new WeightedTierName("Muttering", 3),
            new WeightedTierName("Weeping", 2),
            new WeightedTierName("Wailing", 0.5),
            new WeightedTierName("Screaming", 0.5),
            new WeightedTierName("Shrieking", 2),
            new WeightedTierName("Deafening", 3),
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
            return $"{ plusMinus }{ valueRange1 }% summon damage";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.Player.GetDamage<SummonDamageClass>() += Type1.GetValue();
        }
    }
}
