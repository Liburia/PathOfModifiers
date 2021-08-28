using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using PathOfModifiers.Projectiles;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class ChestKnockbackImmunity : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(2f, 4f, 3),
                new TTFloat.WeightedTier(4f, 6f, 2.5),
                new TTFloat.WeightedTier(6f, 8f, 2),
                new TTFloat.WeightedTier(8f, 10f, 1.5),
                new TTFloat.WeightedTier(10f, 12f, 1),
                new TTFloat.WeightedTier(12f, 14f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(15f, 13f, 3),
                new TTFloat.WeightedTier(13f, 11f, 2.5),
                new TTFloat.WeightedTier(11f, 9f, 2),
                new TTFloat.WeightedTier(9f, 7f, 1.5),
                new TTFloat.WeightedTier(7f, 5f, 1),
                new TTFloat.WeightedTier(5f, 3f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Firmness", 0.5),
            new WeightedTierName("of Perseverance", 1),
            new WeightedTierName("of Endurance", 1.5),
            new WeightedTierName("of Tenacity", 2),
            new WeightedTierName("of Unwavering", 2.5),
            new WeightedTierName("of Unyielding", 3),
        };

        public uint lastProcTime = 0;

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsBodyArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(1), Type1.GetMinValueFormat(1), Type1.GetMaxValueFormat(1), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(1), Type2.GetMinValueFormat(1), Type2.GetMaxValueFormat(1), useChatTags);
            return $"Gain knockback immunity for { valueRange1 }s when hit ({ valueRange2 }s CD)";
        }

        public override void PostHurt(Item item, Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (ItemItem.IsArmorEquipped(item, player) && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type2.GetValue() * 60))
            {
                player.GetModPlayer<BuffPlayer>().AddKnockbackImmunityBuff(player, (int)Math.Round(Type1.GetValue() * 60), false);

                lastProcTime = Main.GameUpdateCount;
            }
        }

        public override Affix Clone()
        {
            var affix = (ChestKnockbackImmunity)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}