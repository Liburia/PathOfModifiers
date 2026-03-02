using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class AccessoryFlatCritChance: AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.07f, -0.05f, 0.5),
                new TTFloat.WeightedTier(-0.05f, -0.03f, 1),
                new TTFloat.WeightedTier(-0.03f, -0.01f, 2),
                new TTFloat.WeightedTier(0.01f, 0.03f, 2),
                new TTFloat.WeightedTier(0.03f, 0.05f, 1),
                new TTFloat.WeightedTier(0.05f, 0.07f, 0.5),
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
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return Language.GetText("Mods.PathOfModifiers.Affixes.Prefixes.AccessoryFlatCritChance").Format(plusMinus, valueRange1);
        }

        public override void PlayerModifyWeaponCrit(Item item, Item heldItem, Player player, ref StatModifier multiplier)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
            {
                float value = Type1.GetValue();
                multiplier.Base += value * 100f;
            }
        }
    }
}
