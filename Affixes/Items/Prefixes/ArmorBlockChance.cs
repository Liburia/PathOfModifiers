using PathOfModifiers.UI.Chat;
using Terraria;
using Terraria.Localization;
namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class ArmorBlockChance : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.010f, 0.020f, 3),
                new TTFloat.WeightedTier(0.020f, 0.030f, 2.5),
                new TTFloat.WeightedTier(0.030f, 0.040f, 2),
                new TTFloat.WeightedTier(0.040f, 0.050f, 1.5),
                new TTFloat.WeightedTier(0.050f, 0.060f, 1),
                new TTFloat.WeightedTier(0.060f, 0.070f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Covering", 0.5),
            new WeightedTierName("Blocking", 1),
            new WeightedTierName("Fending", 1.5),
            new WeightedTierName("Shielded", 2),
            new WeightedTierName("Secure", 2.5),
            new WeightedTierName("Walled", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            return Language.GetText("Mods.PathOfModifiers.Affixes.Prefixes.ArmorBlockChance").Format( valueRange1, Keyword.GetTextOrTag(KeywordType.Block, useChatTags));
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.blockChance += Type1.GetValue();
        }
    }
}
