using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class HelmetArmorPen : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(1, 3, 3),
                new TTInt.WeightedTier(3, 5, 2.5),
                new TTInt.WeightedTier(5, 7, 2),
                new TTInt.WeightedTier(7, 9, 1.5),
                new TTInt.WeightedTier(9, 11, 1),
                new TTInt.WeightedTier(11, 13, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Pointed", 0.5),
            new WeightedTierName("Intrusive", 1),
            new WeightedTierName("Puncturing", 1.5),
            new WeightedTierName("Penetrating", 2),
            new WeightedTierName("Piercing", 2.5),
            new WeightedTierName("Perforating", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsHeadArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            float valueFormat = Type1.GetCurrentValueFormat();
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 } armor penetration";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.Player.GetArmorPenetration(DamageClass.Generic) += Type1.GetValue();
        }
    }
}
