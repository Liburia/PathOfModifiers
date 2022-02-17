using System;
using Terraria;
using Terraria.DataStructures;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class ArmorGainLife : AffixTiered<TTInt, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(-30, -20, 0.5),
                new TTInt.WeightedTier(-20, -10, 1.5),
                new TTInt.WeightedTier(-10, 1, 3),
                new TTInt.WeightedTier(1, 11, 3),
                new TTInt.WeightedTier(11, 21, 1.5),
                new TTInt.WeightedTier(21, 31, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(17f, 15f, 3),
                new TTFloat.WeightedTier(15f, 13f, 2.5),
                new TTFloat.WeightedTier(13f, 11f, 2),
                new TTFloat.WeightedTier(11f, 9f, 1.5),
                new TTFloat.WeightedTier(9f, 7f, 1),
                new TTFloat.WeightedTier(7f, 5f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Death", 0.5),
            new WeightedTierName("of Enervation", 1),
            new WeightedTierName("of Indolence", 1.5),
            new WeightedTierName("of Verve", 2),
            new WeightedTierName("of Vitality", 2.5),
            new WeightedTierName("of Life", 3),
        };

        public uint lastProcTime = 0;

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(1), Type2.GetMinValueFormat(1), Type2.GetMaxValueFormat(1), useChatTags);
            string gainLose = Type1.GetValue() > 0 ? "Gain" : "Lose";
            return $"{ gainLose } { valueRange1 } life when hit ({ valueRange2 }s CD))";
        }

        public override void PostHurt(Item item, Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            Heal(item, player);
        }

        void Heal(Item item, Player player)
        {
            if (ItemItem.IsArmorEquipped(item, player) && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type2.GetValue() * 60))
            {
                lastProcTime = Main.GameUpdateCount;
                int amount = Type1.GetValue();
                if (amount > 0)
                {
                    player.statLife += amount;
                    PoMEffectHelper.Heal(player, amount);
                }
                else
                {
                    player.immune = false;
                    player.Hurt(PlayerDeathReason.ByPlayer(player.whoAmI), -amount, 0, false);
                }
            }
        }

        public override Affix Clone()
        {
            var affix = (ArmorGainLife)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}