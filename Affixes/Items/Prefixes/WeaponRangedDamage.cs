﻿using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class WeaponRangedDamage : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 0.8;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.50f, -0.37f, 0.5),
                new TTFloat.WeightedTier(-0.37f, -0.23f, 1),
                new TTFloat.WeightedTier(-0.23f, -0.10f, 2),
                new TTFloat.WeightedTier(0.10f, 0.23f, 2),
                new TTFloat.WeightedTier(0.23f, 0.37f, 1),
                new TTFloat.WeightedTier(0.37f, 0.50f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Terrible", 3),
            new WeightedTierName("Defective", 2),
            new WeightedTierName("Imprecise", 0.5),
            new WeightedTierName("Sighted", 0.5),
            new WeightedTierName("Staunch", 2),
            new WeightedTierName("Unreal", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item) &&
                ItemItem.IsRanged(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 }% ranged damage";
        }

        public override void ModifyWeaponDamage(Item item, Player player, ref StatModifier damage)
        {
            float value = Type1.GetValue();
            damage += value;
        }
    }
}
