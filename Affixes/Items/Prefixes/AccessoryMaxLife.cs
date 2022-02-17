using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryMaxLife : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(-20, -14, 0.5),
                new TTInt.WeightedTier(-14, -7, 1),
                new TTInt.WeightedTier(-7, 0, 2),
                new TTInt.WeightedTier(1, 8, 2),
                new TTInt.WeightedTier(8, 15, 1),
                new TTInt.WeightedTier(15, 21, 0.5),
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
                ItemItem.IsAccessory(item);
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
