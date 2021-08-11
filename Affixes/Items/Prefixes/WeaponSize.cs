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
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.3f, 0.5),
                new TTFloat.WeightedTier(-0.2f, 1),
                new TTFloat.WeightedTier(-0.1f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(0.1f, 1),
                new TTFloat.WeightedTier(0.2f, 0.5),
                new TTFloat.WeightedTier(0.3f, 0),
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

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item) &&
                ItemItem.CanMelee(item) &&
                (ItemItem.IsSwinging(item) || ItemItem.IsStabbing(item));
        }

        public override string GetTolltipText()
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();

            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat }% size";
        }

        public override void UseItem(Item item, Player player)
        {
            float value = Type1.GetValue();
            item.scale = baseScale * (value + 1);
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

        public override Affix Clone()
        {
            WeaponSize newAffix = (WeaponSize)base.Clone();
            newAffix.baseScale = baseScale;
            return newAffix;
        }
    }
}
