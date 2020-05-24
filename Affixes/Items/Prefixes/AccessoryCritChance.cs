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
    public class AccessoryCritChance : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight => 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.85f, 0.5),
                new TTFloat.WeightedTier(0.92f, 3),
                new TTFloat.WeightedTier(1f, 3),
                new TTFloat.WeightedTier(1.08f, 0.5),
                new TTFloat.WeightedTier(1.15f, 0),
            },
        };

        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Perfunctory", 4),
            new WeightedTierName("Tepid", 1.5),
            new WeightedTierName("Keen", 1.5),
            new WeightedTierName("Fervent", 4),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsAccessory(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"x{Type1.GetValueFormat(1)} critical strike chance";
        }

        public override void PlayerGetWeaponCrit(Item item, Item heldItem, Player player, ref float multiplier)
        {
            if (PoMItem.IsAccessoryEquipped(item, player))
            {
                multiplier += Type1.GetValue() - 1;
            }
        }
    }
}
