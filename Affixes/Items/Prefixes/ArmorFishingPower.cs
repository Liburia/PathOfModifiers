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
    public class ArmorFishingPower : AffixTiered<TTInt>, IPrefix
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
                new TTInt.WeightedTier(4, 2.5),
                new TTInt.WeightedTier(7, 2),
                new TTInt.WeightedTier(10, 1.5),
                new TTInt.WeightedTier(13, 1),
                new TTInt.WeightedTier(16, 0.5),
                new TTInt.WeightedTier(19, 0),
    },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Grabbing", 0.5),
            new WeightedTierName("Snatching", 1),
            new WeightedTierName("Hooking", 1.5),
            new WeightedTierName("Fishing", 2),
            new WeightedTierName("Lassoing", 2.5),
            new WeightedTierName("Corraling", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetTolltipText()
        {
            int value = Type1.GetValueFormat();
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ value } fishing power";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.Player.fishingSkill += Type1.GetValue();
        }
    }
}
