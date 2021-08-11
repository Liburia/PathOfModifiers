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
    public class HelmetLowHPDamageTaken : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.2f, 3),
                new TTFloat.WeightedTier(0.13f, 2.5),
                new TTFloat.WeightedTier(0.06f, 2),
                new TTFloat.WeightedTier(0f, 1.5),
                new TTFloat.WeightedTier(-0.06f, 1),
                new TTFloat.WeightedTier(-0.13f, 0.5),
                new TTFloat.WeightedTier(-0.2f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Fear", 3),
            new WeightedTierName("of Cowardice", 2),
            new WeightedTierName("of Nerves", 0.5),
            new WeightedTierName("of Bravery", 0.5),
            new WeightedTierName("of Courage", 2),
            new WeightedTierName("of Valor", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsHeadArmor(item);
        }

        public override string GetTolltipText()
        {
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"Take { plusMinus }{ Type1.GetValueFormat() }% damage on low HP";
        }

        public override bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (ItemItem.IsArmorEquipped(item, player) && PoMUtil.IsLowHP(player))
            {
                damageMultiplier += Type1.GetValue();
            }

            return true;
        }
    }
}