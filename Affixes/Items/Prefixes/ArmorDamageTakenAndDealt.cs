using Terraria;
using Terraria.DataStructures;

namespace PathOfModifiers.Affixes.Items.Prefixes
{
    public class ArmorDamageTakenAndDealt : AffixTiered<TTFloat>, IPrefix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.20f, -0.15f, 0.5),
                new TTFloat.WeightedTier(-0.15f, -0.10f, 1),
                new TTFloat.WeightedTier(-0.10f, -0.05f, 2),
                new TTFloat.WeightedTier(0.05f, 0.10f, 2),
                new TTFloat.WeightedTier(0.10f, 0.15f, 1),
                new TTFloat.WeightedTier(0.15f, 0.20f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("Discordant", 3),
            new WeightedTierName("Tense", 1.5),
            new WeightedTierName("Balanced", 0.5),
            new WeightedTierName("Tranquil", 0.5),
            new WeightedTierName("Harmonic", 1.5),
            new WeightedTierName("Concordant", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            char plusMinus = Type1.GetValue() < 0 ? '-' : '+';
            return $"Take and deal { plusMinus }{ valueRange1 }% damage";
        }

        public override void PreHurt(Item item, Player player, ref float damageMultiplier, ref Player.HurtModifiers modifiers)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
            {
                damageMultiplier += Type1.GetValue();
            }
        }
        public override void PlayerModifyHitNPC(Item affixItem, Player player, Item item, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref float critDamageMultiplier, ref NPC.HitModifiers modifiers)
        {
            if (ItemItem.IsAccessoryEquipped(affixItem, player))
            {
                damageMultiplier += Type1.GetValue();
            }
        }
        public override void PlayerModifyHitPvp(Item affixItem, Player player, Item item, Player target, ref float damageMultiplier, ref float critDamageMultiplier, ref Player.HurtModifiers modifiers)
        {
            if (ItemItem.IsAccessoryEquipped(affixItem, player))
            {
                damageMultiplier += Type1.GetValue();
            }
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref float critDamageMultiplier, ref NPC.HitModifiers modifiers)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
            {
                damageMultiplier += Type1.GetValue();
            }
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref float critDamageMultiplier, ref Player.HurtModifiers modifiers)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
            {
                damageMultiplier += Type1.GetValue();
            }
        }
    }
}
