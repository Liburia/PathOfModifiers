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
    public class HelmetArmorPen : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(1, 3, 3),
                new TTInt.WeightedTier(3, 5, 2.5),
                new TTInt.WeightedTier(5, 7, 2),
                new TTInt.WeightedTier(7, 9, 1.5),
                new TTInt.WeightedTier(9, 11, 1),
                new TTInt.WeightedTier(11, 13, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Pointed", 0.5),
            new WeightedTierName("Intrusive", 1),
            new WeightedTierName("Puncturing", 1.5),
            new WeightedTierName("Penetrating", 2),
            new WeightedTierName("Piercing", 2.5),
            new WeightedTierName("Perforating", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsHeadArmor(item);
        }

        public override string GetTolltipText()
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();
            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat } armor penetration";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.Player.armorPenetration += Type1.GetValue();
        }
    }
}
