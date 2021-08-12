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
    public class AccessoryManaSpend : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.1f, 0.09f, 3),
                new TTFloat.WeightedTier(0.09f, 0.08f, 2.5),
                new TTFloat.WeightedTier(0.08f, 0.07f, 2),
                new TTFloat.WeightedTier(0.07f, 0.06f, 1.5),
                new TTFloat.WeightedTier(0.06f, 0.05f, 1),
                new TTFloat.WeightedTier(0.05f, 0.04f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, -0.016f, 3),
                new TTFloat.WeightedTier(-0.016f, -0.033f, 2.5),
                new TTFloat.WeightedTier(-0.033f, -0.05f, 2),
                new TTFloat.WeightedTier(-0.05f, -0.066f, 1.5),
                new TTFloat.WeightedTier(-0.066f, -0.084f, 1),
                new TTFloat.WeightedTier(-0.084f, -0.1f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Simplicity", 0.5),
            new WeightedTierName("of Stupidity", 1),
            new WeightedTierName("of Nonsense", 1.5),
            new WeightedTierName("of Absurdity", 2),
            new WeightedTierName("of Puerility", 2.5),
            new WeightedTierName("of Lunacy", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetTolltipText()
        {
            return $"Spend { Type1.GetValueFormat() }% mana to reduce damage taken by { Type2.GetValueFormat() }%";
        }

        public override bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (ItemItem.IsAccessoryEquipped(item, player) && TryConsumeMana(player))
            {
                damageMultiplier += Type2.GetValue();
            }

            return true;
        }

        bool TryConsumeMana(Player player)
        {
            int amount = (int)Math.Round(player.statManaMax2 * Type1.GetValue());
            return player.CheckMana(amount, true);
        }
    }
}