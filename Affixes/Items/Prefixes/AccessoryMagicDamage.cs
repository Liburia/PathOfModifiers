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
                new TTFloat.WeightedTier(-0.1f, -0.07f, 0.5),
                new TTFloat.WeightedTier(-0.07f, -0.04f, 1),
                new TTFloat.WeightedTier(-0.04f, -0.01f, 2),
                new TTFloat.WeightedTier(0.01f, 0.04f, 2),
                new TTFloat.WeightedTier(0.04f, 0.07f, 1),
                new TTFloat.WeightedTier(0.07f, 0.1f, 0.5),
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

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 }% magic damage";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.Player.GetDamage<MagicDamageClass>() += Type1.GetValue();
        }
    }
}
