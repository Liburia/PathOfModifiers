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
    public class AccessoryLifeRegen : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TierType<float>.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.1f, 0.5),
                new TTFloat.WeightedTier(-0.066f, 1),
                new TTFloat.WeightedTier(-0.033f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(0.033f, 1),
                new TTFloat.WeightedTier(0.066f, 0.5),
                new TTFloat.WeightedTier(0.1f, 0),
            },
        };

        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Fragile", 3),
            new WeightedTierName("Feeble", 2),
            new WeightedTierName("Weak", 0.5),
            new WeightedTierName("Tough", 0.5),
            new WeightedTierName("Healthy", 2),
            new WeightedTierName("Vigorous", 3),
        };


        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();
            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat }% life regen";
        }

        public override void NaturalLifeRegen(Item item, Player player, ref float regenMultiplier)
        {
            if (AffixItemItem.IsAccessoryEquipped(item, player))
            {
                regenMultiplier += Type1.GetValue();
            }
        }
    }
}
