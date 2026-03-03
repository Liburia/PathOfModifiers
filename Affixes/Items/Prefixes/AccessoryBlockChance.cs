using PathOfModifiers.UI.Chat;
using Terraria;
using Terraria.Localization;
namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryBlockChance : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.010f, 0.015f, 3),
                new TTFloat.WeightedTier(0.015f, 0.020f, 2.5),
                new TTFloat.WeightedTier(0.020f, 0.025f, 2),
                new TTFloat.WeightedTier(0.025f, 0.030f, 1.5),
                new TTFloat.WeightedTier(0.030f, 0.035f, 1),
                new TTFloat.WeightedTier(0.035f, 0.040f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Steadfast", 0.5),
            new WeightedTierName("Adamant", 1),
            new WeightedTierName("Warding", 1.5),
            new WeightedTierName("Unwavering", 2),
            new WeightedTierName("Enduring", 2.5),
            new WeightedTierName("Unyielding", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            return Language.GetText("Mods.PathOfModifiers.Affixes.Prefixes.AccessoryBlockChance").Format( valueRange1, Keyword.GetTextOrTag(KeywordType.Block, useChatTags));
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.blockChance += Type1.GetValue();
        }
    }
}
