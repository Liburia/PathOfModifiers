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
    public class EquipMaxLife : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight => 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTInt.WeightedTier[]
    {
                new TTInt.WeightedTier(-20, 0.5),
                new TTInt.WeightedTier(-12, 1.2),
                new TTInt.WeightedTier(-5, 2),
                new TTInt.WeightedTier(0, 2),
                new TTInt.WeightedTier(5, 1),
                new TTInt.WeightedTier(12, 0.5),
                new TTInt.WeightedTier(20, 0),
    },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Slumped", 3),
            new WeightedTierName("Depreciated", 1.5),
            new WeightedTierName("Undermined", 0.5),
            new WeightedTierName("Rotund", 0.5),
            new WeightedTierName("Virile", 1.5),
            new WeightedTierName("Impregnable", 3),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsAnyArmor(item) ||
                PoMItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            int value = Type1.GetValue();
            return $"{(value < 0 ? '-' : '+')}{Math.Abs(value)} max life";
        }

        public override void UpdateEquip(Item item, PoMPlayer player)
        {
            player.player.statLifeMax2 += Type1.GetValue();
        }
    }
}
