using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using PathOfModifiers.Projectiles;
using Terraria.ID;
using PathOfModifiers.ModNet.PacketHandlers;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class EquipGoldDrop : AffixTiered<TTFloat, TTInt>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
    {
                new TTFloat.WeightedTier(0, 3),
                new TTFloat.WeightedTier(0.003f, 2.5),
                new TTFloat.WeightedTier(0.007f, 2),
                new TTFloat.WeightedTier(0.01f, 1.5),
                new TTFloat.WeightedTier(0.013f, 1),
                new TTFloat.WeightedTier(0.017f, 0.5),
                new TTFloat.WeightedTier(0.02f, 0),
    },
        };
        public override TTInt Type2 { get; } = new TTInt()
        {
            TwoWay = false,
            IsRange = false,
            Tiers = new TTInt.WeightedTier[]
    {
                new TTInt.WeightedTier(1, 3),
                new TTInt.WeightedTier(2, 2.5),
                new TTInt.WeightedTier(3, 2),
                new TTInt.WeightedTier(4, 1.5),
                new TTInt.WeightedTier(5, 1),
                new TTInt.WeightedTier(6, 0.5)
    },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of test1", 0.5),
            new WeightedTierName("of test2", 1),
            new WeightedTierName("of test3", 1.5),
            new WeightedTierName("of test4", 2),
            new WeightedTierName("of test5", 2.5),
            new WeightedTierName("of test6", 3),
        };

        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsAnyArmor(item) ||
                PoMItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{Type1.GetValueFormat()}% chance to drop {Type2.GetValue()} gold on kill";
        }

        public override void UpdateEquip(Item item, PoMPlayer player)
        {
            player.player.GetModPlayer<ItemAffixPlayer>().goldDropChances.AddOrUpdate(this, Type1.GetValue(), Type2.GetValue());
        }
    }
}