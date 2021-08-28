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
    public class AccessoryCritChance : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.150f, -0.115f, 0.5),
                new TTFloat.WeightedTier(-0.115f, -0.085f, 1),
                new TTFloat.WeightedTier(-0.085f, -0.050f, 2),
                new TTFloat.WeightedTier(0.050f, 0.085f, 2),
                new TTFloat.WeightedTier(0.085f, 0.115f, 1),
                new TTFloat.WeightedTier(0.115f, 0.150f, 0.5),
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


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var current = Type1.GetCurrentValueFormat(1) * Math.Sign(Type1.GetValue()) + 1;
            var min = Type1.GetMinValueFormat(1) * Math.Sign(Type1.GetValue()) + 1;
            var max = Type1.GetMaxValueFormat(1) * Math.Sign(Type1.GetValue()) + 1;
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(current, min, max, useChatTags);
            return $"x{ valueRange1 } critical strike chance";
        }

        public override void PlayerModifyWeaponCrit(Item item, Item heldItem, Player player, ref float multiplier)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
            {
                multiplier += Type1.GetValue();
            }
        }
    }
}
