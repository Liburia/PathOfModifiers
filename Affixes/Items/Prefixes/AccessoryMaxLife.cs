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
    public class AccessoryMaxLife : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            CanBeZero = false,
            TwoWay = false,
            IsRange = true,
            Tiers = new TTInt.WeightedTier[]
            {
                new TTInt.WeightedTier(-20, 0.5),
                new TTInt.WeightedTier(-14, 1),
                new TTInt.WeightedTier(-7, 2),
                new TTInt.WeightedTier(1, 2),
                new TTInt.WeightedTier(8, 1),
                new TTInt.WeightedTier(15, 0.5),
                new TTInt.WeightedTier(21, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Slumped", 3),
            new WeightedTierName("Depreciated", 2),
            new WeightedTierName("Undermined", 0.5),
            new WeightedTierName("Rotund", 0.5),
            new WeightedTierName("Virile", 2),
            new WeightedTierName("Impregnable", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetTolltipText()
        {
            int value = Type1.GetValueFormat();
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ value } max life";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.Player.statLifeMax2 += Type1.GetValue();
        }
    }
}
