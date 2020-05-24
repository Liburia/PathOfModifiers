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
        public override double Weight => 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(1.5f, 0.5),
                new TTFloat.WeightedTier(1.375f, 1.2),
                new TTFloat.WeightedTier(1.2f, 2),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(0.8f, 1),
                new TTFloat.WeightedTier(0.675f, 0.5),
                new TTFloat.WeightedTier(0.5f, 0),
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


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item) &&
                PoMItem.CanCostMana(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{(Type1.GetValue() < 1 ? '-' : '+')}{Type1.GetValueFormat() - 100}% mana cost";
        }

        public override void HoldItem(Item item, Player player)
        {
            player.manaCost = player.manaCost * Type1.GetValue();
        }
    }
}
