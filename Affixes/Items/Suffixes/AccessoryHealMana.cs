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
    public class AccessoryHealMana : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.002f, 0.005f, 3),
                new TTFloat.WeightedTier(0.005f, 0.008f, 2.5),
                new TTFloat.WeightedTier(0.008f, 0.011f, 2),
                new TTFloat.WeightedTier(0.011f, 0.014f, 1.5),
                new TTFloat.WeightedTier(0.014f, 0.017f, 1),
                new TTFloat.WeightedTier(0.017f, 0.020f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Mind", 0.5),
            new WeightedTierName("of Energy", 1),
            new WeightedTierName("of Spirit", 1.5),
            new WeightedTierName("of Anima", 2),
            new WeightedTierName("of Soul", 2.5),
            new WeightedTierName("of Essence", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            return $"Gain { valueRange1 }% mana when hit";
        }

        public override void PostHurt(Item item, Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            Heal(item, player);
        }

        void Heal(Item item, Player player)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
            {
                int amount = (int)MathHelper.Clamp(player.statManaMax2 * Type1.GetValue(), 1, 9999999);
                if (amount > 0)
                {
                    player.statMana += amount;
                    PoMEffectHelper.HealMana(player, amount);
                }
            }
        }
    }
}