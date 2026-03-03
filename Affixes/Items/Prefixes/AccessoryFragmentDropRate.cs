using Terraria;
using Terraria.Localization;using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryFragmentDropRate : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.01f, 0.05f, 3),
                new TTFloat.WeightedTier(0.05f, 0.10f, 2.5),
                new TTFloat.WeightedTier(0.10f, 0.15f, 2),
                new TTFloat.WeightedTier(0.15f, 0.20f, 1.5),
                new TTFloat.WeightedTier(0.20f, 0.25f, 1),
                new TTFloat.WeightedTier(0.25f, 0.30f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Valuable", 0.5),
            new WeightedTierName("Splendid", 1),
            new WeightedTierName("Wealthy", 1.5),
            new WeightedTierName("Sumptuous", 2),
            new WeightedTierName("Prosperous", 2.5),
            new WeightedTierName("Opulent", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            return Language.GetText("Mods.PathOfModifiers.Affixes.Prefixes.AccessoryFragmentDropRate").Format( valueRange1 );
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.fragmentDropMultiplier += Type1.GetValue();
        }
    }
}
