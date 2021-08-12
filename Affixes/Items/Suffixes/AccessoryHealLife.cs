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
    public class AccessoryHealLife : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 0.0016f, 3),
                new TTFloat.WeightedTier(0.0016f, 0.0033f, 2.5),
                new TTFloat.WeightedTier(0.0033f, 0.005f, 2),
                new TTFloat.WeightedTier(0.005f, 0.0066f, 1.5),
                new TTFloat.WeightedTier(0.0066f, 0.0084f, 1),
                new TTFloat.WeightedTier(0.0084f, 0.01f, 0.5),
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

        public override string GetTolltipText()
        {
            return $"Gain { Type1.GetValueFormat() }% life when hit";
        }

        public override void PostHurt(Item item, Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
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