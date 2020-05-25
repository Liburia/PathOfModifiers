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
    public class WeaponSummonDamage : AffixTiered<TTFloat>
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
            new WeightedTierName("Muttering", 3),
            new WeightedTierName("Weeping", 2),
            new WeightedTierName("Wailing", 0.5),
            new WeightedTierName("Screaming", 0.5),
            new WeightedTierName("Shrieking", 2),
            new WeightedTierName("Deafening", 3),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item) &&
                PoMItem.IsSummon(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{(Type1.GetValue() < 1 ? '-' : '+')}{Type1.GetValueFormat() - 100}% summon damage";
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref float multiplier, ref float flat)
        {
            multiplier += Type1.GetValue() - 1;
        }
    }
}
