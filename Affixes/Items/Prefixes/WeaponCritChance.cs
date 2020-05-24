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
    public class WeaponCritChance : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight => 1;

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
            new WeightedTierName("Perfunctory", 0.5),
            new WeightedTierName("Apathetic", 1),
            new WeightedTierName("Tepid", 1.5),
            new WeightedTierName("Keen", 2),
            new WeightedTierName("Zealous", 2.5),
            new WeightedTierName("Fervent", 3),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item) &&
                PoMItem.CanCrit(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"x{Type1.GetValueFormat(1)} critical strike chance";
        }

        public override void GetWeaponCrit(Item item, Player player, ref float multiplier)
        {
            multiplier += Type1.GetValue() - 1;
        }
    }
}
