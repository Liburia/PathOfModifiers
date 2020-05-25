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
    public class ArmorArmorPen : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            //TODO: what the fuck how does this work at all, make this int and check how pen works in vanilla
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(1.3f, 0.5),
                new TTFloat.WeightedTier(1.2f, 1.2),
                new TTFloat.WeightedTier(1.1f, 2),
                new TTFloat.WeightedTier(1f, 2),
                new TTFloat.WeightedTier(0.9f, 1),
                new TTFloat.WeightedTier(0.8f, 0.5),
                new TTFloat.WeightedTier(0.7f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Pointed", 4),
            new WeightedTierName("Intrusive", 2),
            new WeightedTierName("Puncturing", 0.5),
            new WeightedTierName("Penetrating", 0.5),
            new WeightedTierName("Piercing", 2),
            new WeightedTierName("Perforating", 4),
        };


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsHeadArmor(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{(Type1.GetValue() < 0 ? '-' : '+')}{Type1.GetValueFormat(1)} armor penetration";
        }

        public override void UpdateEquip(Item item, PoMPlayer player)
        {
            player.player.armorPenetration += (int)Type1.GetValue();
        }
    }
}
