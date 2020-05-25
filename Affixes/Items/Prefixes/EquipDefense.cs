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
    public class EquipDefense : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = false,
            Tiers = new TTInt.WeightedTier[]
    {
                new TTInt.WeightedTier(-3, 0.5),
                new TTInt.WeightedTier(-2, 1.2),
                new TTInt.WeightedTier(-1, 2),
                new TTInt.WeightedTier(1, 2),
                new TTInt.WeightedTier(2, 1),
                new TTInt.WeightedTier(3, 0.5),
                new TTInt.WeightedTier(4, 0),
    },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Soft", 4),
            new WeightedTierName("Loose", 2),
            new WeightedTierName("Weakened", 0.5),
            new WeightedTierName("Studded", 0.5),
            new WeightedTierName("Layered", 2),
            new WeightedTierName("Reinforced", 4),
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
            return $"{(value < 0 ? '-' : '+')}{Math.Abs(value)} defense";
        }

        public override void UpdateEquip(Item item, PoMPlayer player)
        {
            player.player.statDefense += Type1.GetValue();
        }
    }
}
