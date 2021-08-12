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
    public class WeaponManaCost : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.5f, 0.333f, 0.5),
                new TTFloat.WeightedTier(0.333f, 0.166f, 1),
                new TTFloat.WeightedTier(0.166f, 0f, 2),
                new TTFloat.WeightedTier(0f, -0.166f, 2),
                new TTFloat.WeightedTier(-0.166f, -0.333f, 1),
                new TTFloat.WeightedTier(-0.333f, -0.5f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Lavish", 3),
            new WeightedTierName("Reckless", 2),
            new WeightedTierName("Careless", 0.5),
            new WeightedTierName("Compensating", 0.5),
            new WeightedTierName("Retaining", 2),
            new WeightedTierName("Preserving", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item) &&
                ItemItem.CanCostMana(item);
        }

        public override string GetTolltipText()
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();

            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat }% mana cost";
        }

        public override void ModifyManaCost(Item item, Player player, ref float reduce, ref float mult)
        {
            float value = Type1.GetValue();
            mult += value;
        }
    }
}
