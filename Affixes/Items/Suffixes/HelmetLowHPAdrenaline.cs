using PathOfModifiers.Buffs;
using Terraria.Localization;using PathOfModifiers.UI.Chat;
using System;
using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class HelmetLowHPAdrenaline : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(1f, 4f, 3),
                new TTFloat.WeightedTier(4f, 7f, 1.5),
                new TTFloat.WeightedTier(7f, 10f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Excitement", 0.5),
            new WeightedTierName("of Motivation", 2),
            new WeightedTierName("of Adrenaline", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsHeadArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(1), Type1.GetMinValueFormat(1), Type1.GetMaxValueFormat(1), useChatTags);
            return Language.GetText("Mods.PathOfModifiers.Affixes.Suffixes.HelmetLowHPAdrenaline").Format( Keyword.GetTextOrTag(KeywordType.Adrenaline, useChatTags) ,  valueRange1 );
        }

        public override void PostHurt(Item item, Player player, Player.HurtInfo info)
        {
            if (ItemItem.IsArmorEquipped(item, player) && PoMUtil.IsLowHP(player))
            {
                player.AddBuff(ModContent.BuffType<Adrenaline>(), (int)Math.Round(Type1.GetValue() * 60));
            }
        }
    }
}