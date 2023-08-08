using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryFishingYield : AffixTiered<TTFloat>, IPrefix
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
            new WeightedTierName("Barren", 3),
            new WeightedTierName("Catchless", 2),
            new WeightedTierName("Unbitey", 0.5),
            new WeightedTierName("Bountiful", 0.5),
            new WeightedTierName("Abundant", 2),
            new WeightedTierName("Teeming", 3),
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
            return $"{ plusMinus }{ valueRange1 }% fishing yield";
        }

        public override void PlayerModifyCaughtFish(Item item, Player player, Item fish, ref float multiplier)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
            {
                multiplier += Type1.GetValue();
            }
        }
    }
}
