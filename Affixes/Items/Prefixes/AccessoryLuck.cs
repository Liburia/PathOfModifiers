using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryLuck : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.200f, -0.150f, 0.5),
                new TTFloat.WeightedTier(-0.150f, -0.100f, 1),
                new TTFloat.WeightedTier(-0.100f, -0.050f, 2),
                new TTFloat.WeightedTier(0.050f, 0.100f, 2),
                new TTFloat.WeightedTier(0.100f, 0.150f, 1),
                new TTFloat.WeightedTier(0.150f, 0.200f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Ill-fated", 3),
            new WeightedTierName("Unlucky", 2),
            new WeightedTierName("Jinxed", 0.5),
            new WeightedTierName("Favored", 0.5),
            new WeightedTierName("Lucky", 2),
            new WeightedTierName("Auspicious", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(1), Type1.GetMinValueFormat(1), Type1.GetMaxValueFormat(1), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 } luck";
        }

        public override void PlayerModifyLuck(Item item, Player player, ref float luck)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
            {
                luck += Type1.GetValue();
            }
        }
    }
}
