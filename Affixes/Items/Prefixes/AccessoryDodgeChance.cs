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
using Terraria.DataStructures;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryDodgeChance : AffixTiered<TTFloat>, IPrefix
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
            new WeightedTierName("Evading", 0.5),
            new WeightedTierName("Dodgy", 1),
            new WeightedTierName("Eluding", 1.5),
            new WeightedTierName("Acrobatic", 2),
            new WeightedTierName("Blurred", 2.5),
            new WeightedTierName("Ghostly", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{Type1.GetValueFormat()}% Dodge chance";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.dodgeChance += Type1.GetValue();
        }
    }
}
