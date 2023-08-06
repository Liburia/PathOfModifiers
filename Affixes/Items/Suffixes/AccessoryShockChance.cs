using PathOfModifiers.UI.Chat;
using Terraria;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class AccessoryShockChance : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.010f, 0.025f, 3),
                new TTFloat.WeightedTier(0.025f, 0.040f, 2.5),
                new TTFloat.WeightedTier(0.040f, 0.055f, 2),
                new TTFloat.WeightedTier(0.055f, 0.070f, 1.5),
                new TTFloat.WeightedTier(0.070f, 0.085f, 1),
                new TTFloat.WeightedTier(0.085f, 0.100f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.010f, 0.025f, 3),
                new TTFloat.WeightedTier(0.025f, 0.040f, 2.5),
                new TTFloat.WeightedTier(0.040f, 0.055f, 2),
                new TTFloat.WeightedTier(0.055f, 0.070f, 1.5),
                new TTFloat.WeightedTier(0.070f, 0.085f, 1),
                new TTFloat.WeightedTier(0.085f, 0.100f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Static", 0.5),
            new WeightedTierName("of Sparking", 1),
            new WeightedTierName("of Discharge", 1.5),
            new WeightedTierName("of Lightning", 2),
            new WeightedTierName("of Electrification", 2.5),
            new WeightedTierName("of Galvanism", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(), Type2.GetMinValueFormat(), Type2.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type2.GetValue() < 0 ? '-' : '+';
            return $"{ valueRange1 }% chance to { Keyword.GetTextOrTag(KeywordType.Shock, useChatTags) }({ plusMinus }{ valueRange2 }%)";
        }

        public override void PlayerOnHitNPC(Item affixItem, Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Shock(item, player, target);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Shock(item, player, target);
        }
        public override void PlayerOnHitPvp(Item affixItem, Player player, Item item, Player target, Player.HurtModifiers modifiers, int damageDone)
        {
            Shock(item, player, target);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, Player.HurtModifiers modifiers, int damageDone)
        {
            Shock(item, player, target);
        }

        void Shock(Item item, Player player, NPC target)
        {
            if (ItemItem.IsAccessoryEquipped(item, player) && Main.rand.NextFloat(1f) < Type1.GetValue())
            {
                target.GetGlobalNPC<BuffNPC>().AddShockedBuff(target, Type2.GetValue(), PoMGlobals.ailmentDuration);
            }
        }
        void Shock(Item item, Player player, Player target)
        {
            if (ItemItem.IsAccessoryEquipped(item, player) && Main.rand.NextFloat(1f) < Type1.GetValue())
            {
                target.GetModPlayer<BuffPlayer>().AddShockedBuff(target, Type2.GetValue(), PoMGlobals.ailmentDuration);
            }
        }
    }
}