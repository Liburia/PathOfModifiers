using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class HelmetBreathEffectiveness : AffixTiered<TTFloat>, IPrefix
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
            new WeightedTierName("Asthmatic", 3),
            new WeightedTierName("Gasping", 2),
            new WeightedTierName("Stertorous", 0.5),
            new WeightedTierName("Breathful", 0.5),
            new WeightedTierName("Lungful", 2),
            new WeightedTierName("Respiratory", 3),
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
            string lessMore = Type1.GetValue() < 0 ? "less" : "more";
            return Language.GetText("Mods.PathOfModifiers.Affixes.Prefixes.HelmetBreathEffectiveness").Format(valueRange1, lessMore);
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            float sign = Math.Sign(Type1.GetValue());
            player.Player.breathEffectiveness *= Type1.GetValue() + sign;
        }
    }
}
