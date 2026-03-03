using Terraria;
using Terraria.Localization;
namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class GreavesJumpHeight : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-1.00f, -0.66f, 0.5),
                new TTFloat.WeightedTier(-0.66f, -0.33f, 1),
                new TTFloat.WeightedTier(-0.33f, -0.05f, 2),
                new TTFloat.WeightedTier(0.05f, 0.33f, 2),
                new TTFloat.WeightedTier(0.33f, 0.66f, 1),
                new TTFloat.WeightedTier(0.66f, 1.00f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Trifling", 3),
            new WeightedTierName("Puny", 2),
            new WeightedTierName("Stunted", 0.5),
            new WeightedTierName("Elevated", 0.5),
            new WeightedTierName("Uplifted", 2),
            new WeightedTierName("Soaring", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsLegArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return Language.GetText("Mods.PathOfModifiers.Affixes.Prefixes.GreavesJumpHeight").Format( plusMinus ,  valueRange1 );
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.jumpHeight += Type1.GetValue();
        }
    }
}
