using Terraria;
using Terraria.Localization;using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class WeaponTileReach : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(-7, -5, 0.5),
                new TTInt.WeightedTier(-5, -3, 1),
                new TTInt.WeightedTier(-3, -1, 2),
                new TTInt.WeightedTier(1, 3, 2),
                new TTInt.WeightedTier(3, 5, 1),
                new TTInt.WeightedTier(5, 7, 0.5),
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
                ItemItem.IsPickaxe(item) || ItemItem.IsHammer(item) || ItemItem.IsAxe(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return Language.GetText("Mods.PathOfModifiers.Affixes.Prefixes.WeaponTileReach").Format( plusMinus ,  valueRange1 );
        }

        public override void UpdateInventory(Item item, ItemPlayer player)
        {
            if (player.Player.HeldItem == item)
            {
                int value = Type1.GetValue();
                Player.tileRangeX += value;
                Player.tileRangeY += value;
            }
        }
    }
}
