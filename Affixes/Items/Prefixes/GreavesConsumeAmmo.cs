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
using System.Data.Odbc;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class GreavesConsumeAmmo : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.05f, 2.5),
                new TTFloat.WeightedTier(0.1f, 2),
                new TTFloat.WeightedTier(0.15f, 1.5),
                new TTFloat.WeightedTier(0.2f, 1),
                new TTFloat.WeightedTier(0.25f, 0.5),
                new TTFloat.WeightedTier(0.3f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Wasteful", 0.5),
            new WeightedTierName("Stable", 1),
            new WeightedTierName("Frugal", 1.5),
            new WeightedTierName("Prudent", 2),
            new WeightedTierName("Conserving", 2.5),
            new WeightedTierName("Materialistic", 3),
        };


        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsLegArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            float valueFormat = Type1.GetValueFormat();
            return $"{ valueFormat }% chance to not consume ammo";
        }

        public override bool PlayerConsumeAmmo(Player player, Item item, Item ammo, ref float chanceToNotConsume)
        {
            if (AffixItemItem.IsArmorEquipped(item, player))
            {
                float value = Type1.GetValue();
                chanceToNotConsume += value;
            }
            return true;
        }
    }
}
