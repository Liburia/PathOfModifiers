using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryMeleeDamage : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.1f, 0.5),
                new TTFloat.WeightedTier(-0.066f, 1),
                new TTFloat.WeightedTier(-0.033f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(0.033f, 1),
                new TTFloat.WeightedTier(0.066f, 0.5),
                new TTFloat.WeightedTier(0.1f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Broken", 3),
            new WeightedTierName("Damaged", 2),
            new WeightedTierName("Dull", 0.5),
            new WeightedTierName("Polished", 0.5),
            new WeightedTierName("Sharp", 2),
            new WeightedTierName("Flaring", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();
            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat }% melee damage";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.Player.GetDamage<MeleeDamageClass>() += Type1.GetValue();
        }
    }
}
