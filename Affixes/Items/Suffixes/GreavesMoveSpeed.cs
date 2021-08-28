using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using PathOfModifiers.Projectiles;
using Terraria.ID;
using PathOfModifiers.ModNet.PacketHandlers;
using Terraria.DataStructures;
using PathOfModifiers.Buffs;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class GreavesMoveSpeed : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.60f, -0.42f, 0.5),
                new TTFloat.WeightedTier(-0.42f, -0.24f, 1),
                new TTFloat.WeightedTier(-0.24f, -0.06f, 2),
                new TTFloat.WeightedTier(0.06f, 0.24f, 2),
                new TTFloat.WeightedTier(0.24f, 0.42f, 1),
                new TTFloat.WeightedTier(0.42f, 0.60f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
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
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(8f, 7f, 3),
                new TTFloat.WeightedTier(7f, 6f, 2.5),
                new TTFloat.WeightedTier(6f, 5f, 2),
                new TTFloat.WeightedTier(5f, 4f, 1.5),
                new TTFloat.WeightedTier(4f, 3f, 1),
                new TTFloat.WeightedTier(3f, 2f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Spurring", 0.5),
            new WeightedTierName("of Hurry", 1),
            new WeightedTierName("of Quickening", 1.5),
            new WeightedTierName("of Haste", 2),
            new WeightedTierName("of Stimulation", 2.5),
            new WeightedTierName("of Rush", 3),
        };

        uint lastProcTime;

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsLegArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(1), Type2.GetMinValueFormat(1), Type2.GetMaxValueFormat(1), useChatTags);
            var valueRange3 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type3.GetCurrentValueFormat(1), Type3.GetMinValueFormat(1), Type3.GetMaxValueFormat(1), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 }% move speed for { valueRange2 }s when hit ({ valueRange3 }s CD)";
        }

        public override void PostHurt(Item item, Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (ItemItem.IsArmorEquipped(item, player) && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type3.GetValue() * 60))
            {
                player.GetModPlayer<BuffPlayer>().AddGreavesMoveSpeedBuff(player, Type1.GetValue(), (int)Math.Round(Type2.GetValue() * 60), false);

                lastProcTime = Main.GameUpdateCount;
            }
        }

        public override Affix Clone()
        {
            var affix = (GreavesMoveSpeed)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}