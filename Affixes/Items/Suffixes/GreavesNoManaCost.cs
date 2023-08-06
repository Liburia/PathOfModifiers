using System;
using Terraria;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class GreavesNoManaCost : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(1f, 2f, 3),
                new TTFloat.WeightedTier(2f, 3f, 2.5),
                new TTFloat.WeightedTier(3f, 4f, 2),
                new TTFloat.WeightedTier(4f, 5f, 1.5),
                new TTFloat.WeightedTier(5f, 6f, 1),
                new TTFloat.WeightedTier(6f, 7f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(10f, 9f, 3),
                new TTFloat.WeightedTier(9f, 8f, 2.5),
                new TTFloat.WeightedTier(8f, 7f, 2),
                new TTFloat.WeightedTier(7f, 6f, 1.5),
                new TTFloat.WeightedTier(6f, 5f, 1),
                new TTFloat.WeightedTier(5f, 4f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Obscurity", 0.5),
            new WeightedTierName("of Mystery", 1),
            new WeightedTierName("of Mystic", 1.5),
            new WeightedTierName("of Cabal", 2),
            new WeightedTierName("of Occult", 2.5),
            new WeightedTierName("of Arcana", 3),
        };

        public uint lastProcTime = 0;

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsLegArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(1), Type1.GetMinValueFormat(1), Type1.GetMaxValueFormat(1), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(1), Type2.GetMinValueFormat(1), Type2.GetMaxValueFormat(1), useChatTags);
            return $"No mana cost for { valueRange1 }s when hit ({ valueRange2 }s CD)";
        }

        public override void PostHurt(Item item, Player player, Player.HurtInfo info)
        {
            if (ItemItem.IsArmorEquipped(item, player) && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type2.GetValue() * 60))
            {
                player.GetModPlayer<BuffPlayer>().AddNoManaCostBuff(player, (int)Math.Round(Type1.GetValue() * 60), false);

                lastProcTime = Main.GameUpdateCount;
            }
        }

        public override Affix Clone()
        {
            var affix = (GreavesNoManaCost)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}