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
using Terraria.DataStructures;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class EquipBlockChance : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
    {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.003f, 2.5),
                new TTFloat.WeightedTier(0.007f, 2),
                new TTFloat.WeightedTier(0.01f, 1.5),
                new TTFloat.WeightedTier(0.013f, 1),
                new TTFloat.WeightedTier(0.017f, 0.5),
                new TTFloat.WeightedTier(0.02f, 0),
    },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Testing 1", 4),
            new WeightedTierName("Testing 2", 2),
            new WeightedTierName("Testing 3", 0.5),
            new WeightedTierName("Testing 4", 0.5),
            new WeightedTierName("Testing 5", 2),
            new WeightedTierName("Testing 6", 4),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsAnyArmor(item) ||
                PoMItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{Type1.GetValueFormat()}% chance to block damage";
        }

        public override void UpdateEquip(Item item, PoMPlayer player)
        {
            //if (PoMItem.IsEquipped(item, player)) dont need this since UpdateEquip
            player.blockChance += Type1.GetValue();
        }
    }
}
