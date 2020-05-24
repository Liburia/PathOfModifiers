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
    public class WeaponSize : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight => 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.7f, 0.5),
                new TTFloat.WeightedTier(0.8f, 1.2),
                new TTFloat.WeightedTier(0.9f, 2),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(1.1f, 1),
                new TTFloat.WeightedTier(1.2f, 0.5),
                new TTFloat.WeightedTier(1.3f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Tiny", 3),
            new WeightedTierName("Small", 2),
            new WeightedTierName("Modest", 0.5),
            new WeightedTierName("Bulky", 0.5),
            new WeightedTierName("Large", 2),
            new WeightedTierName("Massive", 3),
        };


        float baseScale = -1;

        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item) &&
                PoMItem.CanMelee(item) &&
                (PoMItem.IsSwinging(item) || PoMItem.IsStabbing(item));
        }

        public override string GetTolltipText(Item item)
        {
            return $"{(Type1.GetValue() < 1 ? '-' : '+')}{Type1.GetValueFormat() - 100}% size";
        }

        public override void UseItem(Item item, Player player)
        {
            item.scale = baseScale * Type1.GetValue();
        }

        /// <summary>
        /// Called when this affix is added to an item
        /// </summary>
        public override void AddAffix(Item item, bool clone)
        {
            if (!clone)
                baseScale = item.scale;
        }
        /// <summary>
        /// Called when this affix is removed from an item
        /// </summary>
        public override void RemoveAffix(Item item)
        {
            item.scale = baseScale;
        }
    }
}
