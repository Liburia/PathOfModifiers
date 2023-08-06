using Microsoft.Xna.Framework;
using Terraria;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class AccessoryHealLife : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.0010f, 0.0025f, 3),
                new TTFloat.WeightedTier(0.0025f, 0.0040f, 2.5),
                new TTFloat.WeightedTier(0.0040f, 0.0055f, 2),
                new TTFloat.WeightedTier(0.0055f, 0.0070f, 1.5),
                new TTFloat.WeightedTier(0.0070f, 0.0085f, 1),
                new TTFloat.WeightedTier(0.0085f, 0.0100f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Haleness", 0.5),
            new WeightedTierName("of Well-being", 1),
            new WeightedTierName("of Health", 1.5),
            new WeightedTierName("of Constitution", 2),
            new WeightedTierName("of Vigor", 2.5),
            new WeightedTierName("of Vermilion", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            return $"Gain { valueRange1 }% max life when hit";
        }

        public override void PostHurt(Item item, Player player, Player.HurtInfo info)
        {
            Heal(item, player);
        }

        void Heal(Item item, Player player)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
            {
                int amount = (int)MathHelper.Clamp(player.statLifeMax2 * Type1.GetValue(), 1, 9999999);
                if (amount > 0)
                {
                    player.statLife += amount;
                    PoMEffectHelper.Heal(player, amount);
                }
            }
        }
    }
}