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
    public class ArmorMaxLife : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            CanBeZero = false,
            TwoWay = false,
            IsRange = true,
            Tiers = new TTInt.WeightedTier[]
    {
                new TTInt.WeightedTier(-30, 0.5),
                new TTInt.WeightedTier(-20, 1),
                new TTInt.WeightedTier(-10, 2),
                new TTInt.WeightedTier(1, 2),
                new TTInt.WeightedTier(11, 1),
                new TTInt.WeightedTier(21, 0.5),
                new TTInt.WeightedTier(31, 0),
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


        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsAnyArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            int value = Type1.GetValueFormat();
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ value } max life";
        }

        public override void UpdateEquip(Item item, AffixItemPlayer player)
        {
            player.player.statLifeMax2 += Type1.GetValue();
        }
    }
}
