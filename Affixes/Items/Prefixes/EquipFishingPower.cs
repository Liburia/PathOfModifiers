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
    public class EquipFishingPower : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTInt.WeightedTier[]
    {
                new TTInt.WeightedTier(0, 3),
                new TTInt.WeightedTier(2, 2.5),
                new TTInt.WeightedTier(4, 2),
                new TTInt.WeightedTier(6, 1.5),
                new TTInt.WeightedTier(8, 1),
                new TTInt.WeightedTier(10, 0.5),
                new TTInt.WeightedTier(12, 0),
    },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Testin 1", 4),
            new WeightedTierName("Testin 2", 2),
            new WeightedTierName("Testin 3", 0.5),
            new WeightedTierName("Testin 4", 0.5),
            new WeightedTierName("Testin 5", 2),
            new WeightedTierName("Testin 6", 4),
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
            return $"{(value < 0 ? '-' : '+')}{Math.Abs(value)} fishing power";
        }

        public override void UpdateEquip(Item item, PoMPlayer player)
        {
            player.player.fishingSkill += Type1.GetValue();
        }
    }
}
