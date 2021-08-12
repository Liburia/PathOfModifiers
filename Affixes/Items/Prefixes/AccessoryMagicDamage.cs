using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryMagicDamage : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.1f, -0.066f, 0.5),
                new TTFloat.WeightedTier(-0.066f, -0.033f, 1),
                new TTFloat.WeightedTier(-0.033f, 0f, 2),
                new TTFloat.WeightedTier(0f, 0.033f, 2),
                new TTFloat.WeightedTier(0.033f, 0.066f, 1),
                new TTFloat.WeightedTier(0.066f, 0.1f, 0.5),
            },
        };

        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Deranged", 3),
            new WeightedTierName("Ignorant", 2),
            new WeightedTierName("Inept", 0.5),
            new WeightedTierName("Adept", 0.5),
            new WeightedTierName("Mystic", 2),
            new WeightedTierName("Mythical", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetTolltipText()
        {
            float value = Type1.GetValue();
            float valueFormat = Type1.GetValueFormat();
            char plusMinus = value < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueFormat }% magic damage";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.Player.GetDamage<MagicDamageClass>() += Type1.GetValue();
        }
    }
}
