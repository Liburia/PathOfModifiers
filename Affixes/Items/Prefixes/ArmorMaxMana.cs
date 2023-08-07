using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class ArmorMaxMana : AffixTiered<TTInt>, IPrefix
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
            new WeightedTierName("Drab", 3),
            new WeightedTierName("Dreary", 2),
            new WeightedTierName("Faded", 0.5),
            new WeightedTierName("Sapphire", 0.5),
            new WeightedTierName("Azure", 2),
            new WeightedTierName("Opalescent", 3),
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
            return $"{ plusMinus }{ valueRange1 } max mana";
        }

        public override void PlayerModifyMaxStats(Item item, ref StatModifier health, ref StatModifier mana)
        {
            mana.Flat += Type1.GetValue();
        }
    }
}
