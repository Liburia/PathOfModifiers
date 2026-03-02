using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class WeaponProjectileSpeed : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1.0;

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
            new WeightedTierName("Impeding", 3),
            new WeightedTierName("Repressing", 2),
            new WeightedTierName("Restricting", 0.5),
            new WeightedTierName("Lobbing", 0.5),
            new WeightedTierName("Slinging", 2),
            new WeightedTierName("Hurling", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item) &&
                ItemItem.IsShooting(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return Language.GetText("Mods.PathOfModifiers.Affixes.Prefixes.WeaponProjectileSpeed").Format( plusMinus ,  valueRange1 );
        }

        public override void ModifyShootVelocity(Item item, Player player, ref StatModifier multiplier)
        {
            multiplier += Type1.GetValue();
        }
    }
}
