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
    public class WeaponRangedDamage : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 0.8;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.5f, 0.5),
                new TTFloat.WeightedTier(0.675f, 1.2),
                new TTFloat.WeightedTier(0.85f, 2),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(1.2f, 1),
                new TTFloat.WeightedTier(1.375f, 0.5),
                new TTFloat.WeightedTier(1.5f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Terrible", 3),
            new WeightedTierName("Defective", 2),
            new WeightedTierName("Imprecise", 0.5),
            new WeightedTierName("Sighted", 0.5),
            new WeightedTierName("Staunch", 2),
            new WeightedTierName("Unreal", 3),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item) &&
                PoMItem.IsRanged(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{(Type1.GetValue() < 1 ? '-' : '+')}{Type1.GetValueFormat() - 100}% ranged damage";
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref float multiplier, ref float flat)
        {
            multiplier += Type1.GetValue() - 1;
        }
    }
}
