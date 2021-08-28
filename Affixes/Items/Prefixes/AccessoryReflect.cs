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
    //Doesn't work with pvp, no hook. alternatively use on hit pvp hook
    public class AccessoryReflect : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.050f, 0.125f, 3),
                new TTFloat.WeightedTier(0.125f, 0.200f, 2.5),
                new TTFloat.WeightedTier(0.200f, 0.275f, 2),
                new TTFloat.WeightedTier(0.275f, 0.350f, 1.5),
                new TTFloat.WeightedTier(0.350f, 0.425f, 1),
                new TTFloat.WeightedTier(0.425f, 0.500f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Prickly", 0.5),
            new WeightedTierName("Thorny", 1),
            new WeightedTierName("Barbed", 1.5),
            new WeightedTierName("Spiky", 2),
            new WeightedTierName("Bristly", 2.5),
            new WeightedTierName("Spinous", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            return $"{ valueRange1 }% melee damage reflected";
        }

        public override void UpdateEquip(Item item, ItemPlayer player)
        {
            player.reflectMeleeDamage += Type1.GetValue();
        }
    }
}
