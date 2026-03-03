using Terraria;
using Terraria.Localization;using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryTileReach : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = false,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(-3, -3, 0.5),
                new TTInt.WeightedTier(-2, -2, 1),
                new TTInt.WeightedTier(-1, -1, 2),
                new TTInt.WeightedTier(1, 1, 2),
                new TTInt.WeightedTier(2, 2, 1),
                new TTInt.WeightedTier(3, 3, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Terse", 3),
            new WeightedTierName("Pithy", 2),
            new WeightedTierName("Compact", 0.5),
            new WeightedTierName("Lengthy", 0.5),
            new WeightedTierName("Elongated", 2),
            new WeightedTierName("Outstretched", 3),
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
            return Language.GetText("Mods.PathOfModifiers.Affixes.Prefixes.AcessoryTileReach").Format( plusMinus ,  valueRange1 );
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            int value = Type1.GetValue();
            Player.tileRangeX += value;
            Player.tileRangeY += value;
        }
    }
}
