using Terraria;
using Terraria.DataStructures;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class ChestFullHPDamageTaken : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.20f, 0.14f, 0.5),
                new TTFloat.WeightedTier(0.14f, 0.08f, 1),
                new TTFloat.WeightedTier(0.08f, 0.02f, 2),
                new TTFloat.WeightedTier(-0.02f, -0.08f, 2),
                new TTFloat.WeightedTier(-0.08f, -0.14f, 1),
                new TTFloat.WeightedTier(-0.14f, -0.20f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Apprehension", 3),
            new WeightedTierName("of Irresolution", 2),
            new WeightedTierName("of Doubt", 0.5),
            new WeightedTierName("of Grit", 0.5),
            new WeightedTierName("of Resolution", 2),
            new WeightedTierName("of Fortitude", 3),
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
            return $"Take { plusMinus }{ valueRange1 }% damage on full HP";
        }

        public override void PreHurt(Item item, Player player, ref float damageMultiplier, ref Player.HurtModifiers modifiers)
        {
            if (ItemItem.IsArmorEquipped(item, player) && player.statLife == player.statLifeMax2)
            {
                damageMultiplier += Type1.GetValue();
            }
        }
    }
}