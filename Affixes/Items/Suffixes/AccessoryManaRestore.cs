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

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class AccessoryManaRestore : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.04f, 0.05f, 3),
                new TTFloat.WeightedTier(0.05f, 0.06f, 2.5),
                new TTFloat.WeightedTier(0.06f, 0.07f, 2),
                new TTFloat.WeightedTier(0.07f, 0.08f, 1.5),
                new TTFloat.WeightedTier(0.08f, 0.09f, 1),
                new TTFloat.WeightedTier(0.09f, 0.1f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.1f, 0.084f, 3),
                new TTFloat.WeightedTier(0.084f, 0.066f, 2.5),
                new TTFloat.WeightedTier(0.066f, 0.05f, 2),
                new TTFloat.WeightedTier(0.05f, 0.033f, 1.5),
                new TTFloat.WeightedTier(0.033f, 0.016f, 1),
                new TTFloat.WeightedTier(0.016f, 0f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Intuition", 0.5),
            new WeightedTierName("of Insight", 1),
            new WeightedTierName("of Acumen", 1.5),
            new WeightedTierName("of Ascendance", 2),
            new WeightedTierName("of Transcendence", 2.5),
            new WeightedTierName("of Divinity", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(), Type2.GetMinValueFormat(), Type2.GetMaxValueFormat(), useChatTags);
            return $"Restore { valueRange1 }% mana to increase damage taken by { valueRange2 }% when hit";
        }

        public override bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
            {
                int manaAmount = (int)Math.Round(player.statManaMax2 * Type1.GetValue());
                player.statMana += manaAmount;
                PoMEffectHelper.HealMana(player, manaAmount);
                damageMultiplier += Type2.GetValue();
            }

            return true;
        }
    }
}