using Terraria;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponLowHPCrit : AffixTiered<TTFloat>, ISuffix
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
            new WeightedTierName("of Failure", 3),
            new WeightedTierName("of Mercy", 2),
            new WeightedTierName("of Hesitation", 0.5),
            new WeightedTierName("of Carnage", 0.5),
            new WeightedTierName("of Execution", 2),
            new WeightedTierName("of Extermination", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            string plusMinus = Type1.GetValue() >= 0 ? "+" : "-";
            return $"Deal { plusMinus }{ valueRange1 }% damage to low HP enemies";
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref NPC.HitModifiers modifiers)
        {
            NPC realTarget = target.realLife >= 0 ? Main.npc[target.realLife] : target;
            if (item == player.HeldItem && (realTarget.life / (float)realTarget.lifeMax) <= PoMGlobals.lowHPThreshold)
            {
                damageMultiplier += Type1.GetValue();
            }
        }
        public override void ModifyHitPvp(Item item, Player player, Player target, ref float damageMultiplier, ref Player.HurtModifiers modifiers)
        {
            if (item == player.HeldItem && PoMUtil.IsLowHP(target))
            {
                damageMultiplier += Type1.GetValue();
            }
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref float critDamageMultiplier, ref NPC.HitModifiers modifiers)
        {
            NPC realTarget = target.realLife >= 0 ? Main.npc[target.realLife] : target;
            if (item == player.HeldItem && (realTarget.life / (float)realTarget.lifeMax) <= PoMGlobals.lowHPThreshold)
            {
                damageMultiplier += Type1.GetValue();
            }
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref float critDamageMultiplier, ref Player.HurtModifiers modifiers)
        {
            if (item == player.HeldItem && PoMUtil.IsLowHP(target))
            {
                damageMultiplier += Type1.GetValue();
            }
        }
    }
}