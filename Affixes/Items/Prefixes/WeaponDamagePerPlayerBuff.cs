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
    public class WeaponDamagePerPlayerBuff : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight => 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.97f, 0.5),
                new TTFloat.WeightedTier(0.98f, 1.2),
                new TTFloat.WeightedTier(0.99f, 2),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(1.01f, 1),
                new TTFloat.WeightedTier(1.02f, 0.5),
                new TTFloat.WeightedTier(1.03f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Thwarthed", 3),
            new WeightedTierName("Hindered", 2),
            new WeightedTierName("Restrained", 0.5),
            new WeightedTierName("Servicable", 0.5),
            new WeightedTierName("Conducive", 2),
            new WeightedTierName("Serendipitous", 3),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{(Type1.GetValue() < 1 ? '-' : '+')}{Type1.GetValueFormat() - 100}% damage per player buff/debuff";
        }
    }
}
