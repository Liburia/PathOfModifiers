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
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.5f, 0.5),
                new TTFloat.WeightedTier(-0.333f, 1),
                new TTFloat.WeightedTier(-0.166f, 2),
                new TTFloat.WeightedTier(0f, 2),
                new TTFloat.WeightedTier(0.166f, 1),
                new TTFloat.WeightedTier(0.333f, 0.5),
                new TTFloat.WeightedTier(0.5f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Perfunctory", 3),
            new WeightedTierName("Apathetic", 2),
            new WeightedTierName("Tepid", 0.5),
            new WeightedTierName("Keen", 0.5),
            new WeightedTierName("Zealous", 2),
            new WeightedTierName("Fervent", 3),
        };


        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsWeapon(item) &&
                AffixItemItem.CanCrit(item);
        }

        public override string GetTolltipText(Item item)
        {
            float valueFormat = (float)Math.Round(Type1.GetValue() + 1f, 1);
            return $"x{ valueFormat } critical strike chance";
        }

        public override void GetWeaponCrit(Item item, Player player, ref float multiplier)
        {
            float value = Type1.GetValue();
            multiplier += value;
        }
    }
}
