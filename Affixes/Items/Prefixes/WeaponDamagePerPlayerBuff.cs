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

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class WeaponDamagePerPlayerBuff : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.03f, 0.5),
                new TTFloat.WeightedTier(-0.02f, 1),
                new TTFloat.WeightedTier(-0.01f, 2),
                new TTFloat.WeightedTier(0, 2),
                new TTFloat.WeightedTier(0.01f, 1),
                new TTFloat.WeightedTier(0.02f, 0.5),
                new TTFloat.WeightedTier(0.03f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Thwarthed", 3),
            new WeightedTierName("Hindered", 2),
            new WeightedTierName("Restrained", 0.5),
            new WeightedTierName("Servicable", 0.5),
            new WeightedTierName("Conducive", 2),
            new WeightedTierName("Serendipitous", 3),
        };


        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();

            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat }% damage per player buff/debuff";
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref float add, ref float multiplier, ref float flat)
        {
            float value = Type1.GetValue();
            add += value * PoMUtil.CountBuffs(player.buffType);
        }
    }
}
