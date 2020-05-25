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
    public class WeaponDamagePerEnemyBuff : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

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
            new WeightedTierName("Precipitating", 3),
            new WeightedTierName("Expediting", 2),
            new WeightedTierName("Urging", 0.5),
            new WeightedTierName("Impeding", 0.5),
            new WeightedTierName("Styming", 2),
            new WeightedTierName("Thwarthing", 3),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{(Type1.GetValue() < 1 ? '-' : '+')}{Type1.GetValueFormat() - 100}% damage per enemy buff/debuff";
        }
    }
}
