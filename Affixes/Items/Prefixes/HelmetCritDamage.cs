using Terraria;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class HelmetCritDamage : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

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
            new WeightedTierName("Gentle", 3),
            new WeightedTierName("Calm", 2),
            new WeightedTierName("Tame", 0.5),
            new WeightedTierName("Fierce", 0.5),
            new WeightedTierName("Ferocious", 2),
            new WeightedTierName("Savage", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsHeadArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"{ plusMinus }{ valueRange1 }% crit damage";
        }

        public override void PlayerModifyHitNPC(Item affixItem, Player player, Item item, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref float critDamageMultiplier, ref NPC.HitModifiers modifiers)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
                critDamageMultiplier += Type1.GetValue();
        }
        public override void PlayerModifyHitPvp(Item affixItem, Player player, Item item, Player target, ref float damageMultiplier, ref float critDamageMultiplier, ref Player.HurtModifiers modifiers)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
                critDamageMultiplier += Type1.GetValue();
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref float critDamageMultiplier, ref NPC.HitModifiers modifiers)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
                critDamageMultiplier += Type1.GetValue();
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref float critDamageMultiplier, ref Player.HurtModifiers modifiers)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
                critDamageMultiplier += Type1.GetValue();
        }
    }
}
