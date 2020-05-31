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
    public class ArmorDamageTaken : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.15f, 0.5),
                new TTFloat.WeightedTier(0.1f, 1),
                new TTFloat.WeightedTier(0.05f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(-0.05f, 1),
                new TTFloat.WeightedTier(-0.1f, 0.5),
                new TTFloat.WeightedTier(-0.15f, 0),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(17f, 3),
                new TTFloat.WeightedTier(15f, 2.5),
                new TTFloat.WeightedTier(13f, 2),
                new TTFloat.WeightedTier(11f, 1.5),
                new TTFloat.WeightedTier(9f, 1),
                new TTFloat.WeightedTier(7f, 0.5),
                new TTFloat.WeightedTier(5f, 0),
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

        double lastProcTime = 0;

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsAnyArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"Take { Type1.GetValueFormat() }% damage ({ Type2.GetValueFormat(1) }s CD)";
        }

        public override bool PreHurt(Item item, Player player, bool pvp, bool quiet, ref float damageMultiplier, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (AffixItemItem.IsArmorEquipped(item, player) && (PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds - lastProcTime) >= Type2.GetValue() * 1000)
            {
                damageMultiplier += Type1.GetValue();
                lastProcTime = PathOfModifiers.gameTime.TotalGameTime.TotalMilliseconds;
            }

            return true;
        }
    }
}