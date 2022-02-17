using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class ArmorMaxLife : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(-30, -20, 0.5),
                new TTInt.WeightedTier(-20, -10, 1),
                new TTInt.WeightedTier(-10, 0, 2),
                new TTInt.WeightedTier(1, 11, 2),
                new TTInt.WeightedTier(11, 21, 1),
                new TTInt.WeightedTier(21, 31, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Slumped", 3),
            new WeightedTierName("Depreciated", 2),
            new WeightedTierName("Undermined", 0.5),
            new WeightedTierName("Rotund", 0.5),
            new WeightedTierName("Virile", 2),
            new WeightedTierName("Impregnable", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 } max life";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.Player.statLifeMax2 += Type1.GetValue();
        }
    }
}
