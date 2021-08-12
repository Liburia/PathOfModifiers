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
    public class AccessoryDamageTaken : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.1f, -0.07f, 0.5),
                new TTFloat.WeightedTier(-0.07f, -0.04f, 1),
                new TTFloat.WeightedTier(-0.04f, -0.01f, 2),
                new TTFloat.WeightedTier(0.01f, 0.04f, 2),
                new TTFloat.WeightedTier(0.04f, 0.07f, 1),
                new TTFloat.WeightedTier(0.07f, 0.1f, 0.5),
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
            new WeightedTierName("of Discord", 0.5),
            new WeightedTierName("of Tension", 1),
            new WeightedTierName("of Balance", 1.5),
            new WeightedTierName("of Tranquility", 2),
            new WeightedTierName("of Harmony", 2.5),
            new WeightedTierName("of Concord", 3),
        };

        public uint lastProcTime = 0;

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetTolltipText()
        {
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"Take { plusMinus }{ Type1.GetValueFormat() }% damage ({ Type2.GetValueFormat(1) }s CD)";
        }

        public override bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (ItemItem.IsAccessoryEquipped(item, player) && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type2.GetValue() * 60))
            {
                damageMultiplier += Type1.GetValue();
                lastProcTime = Main.GameUpdateCount;
            }

            return true;
        }

        public override Affix Clone()
        {
            var affix = (AccessoryDamageTaken)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}