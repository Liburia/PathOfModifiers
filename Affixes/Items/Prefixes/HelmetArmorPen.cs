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
    public class HelmetArmorPen : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            CanBeZero = false,
            TwoWay = false,
            IsRange = true,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(1, 3),
                new TTInt.WeightedTier(3, 2.5),
                new TTInt.WeightedTier(5, 2),
                new TTInt.WeightedTier(7, 1.5),
                new TTInt.WeightedTier(9, 1),
                new TTInt.WeightedTier(11, 0.5),
                new TTInt.WeightedTier(13, 0),
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


        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsHeadArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();
            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat } armor penetration";
        }

        public override void UpdateEquip(Item item, AffixItemPlayer player)
        {
            player.player.armorPenetration += Type1.GetValue();
        }
    }
}
