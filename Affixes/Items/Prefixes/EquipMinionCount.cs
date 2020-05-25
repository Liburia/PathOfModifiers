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
    public class EquipMinionCount : AffixTiered<TTInt>, IPrefix
    {
        public override double Weight { get; } = 0.6;

        public override TTInt Type1 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = false,
            Tiers = new TTInt.WeightedTier[]
    {
                new TTInt.WeightedTier(1, 10),
                new TTInt.WeightedTier(2, 0.5),
    },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Invoking", 2),
            new WeightedTierName("Conjuring", 4),
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
            return $"{(value < 0 ? '-' : '+')}{Math.Abs(value)} max minions";
        }

        public override void UpdateEquip(Item item, PoMPlayer player)
        {
            player.player.maxMinions += Type1.GetValue();
        }
    }
}
