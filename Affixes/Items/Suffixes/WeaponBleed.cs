using Microsoft.Xna.Framework;
using PathOfModifiers.UI.Chat;
using Terraria;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponBleed : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.10f, 0.25f, 3),
                new TTFloat.WeightedTier(0.25f, 0.40f, 2.5),
                new TTFloat.WeightedTier(0.40f, 0.55f, 2),
                new TTFloat.WeightedTier(0.55f, 0.70f, 1.5),
                new TTFloat.WeightedTier(0.70f, 0.85f, 1),
                new TTFloat.WeightedTier(0.85f, 1.00f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.2f, 0.5f, 3),
                new TTFloat.WeightedTier(0.5f, 0.8f, 2.5),
                new TTFloat.WeightedTier(0.8f, 1.1f, 2),
                new TTFloat.WeightedTier(1.1f, 1.4f, 1.5),
                new TTFloat.WeightedTier(1.4f, 1.7f, 1),
                new TTFloat.WeightedTier(1.7f, 2.0f, 0.5),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(2f, 4f, 3),
                new TTFloat.WeightedTier(4f, 6f, 2.5),
                new TTFloat.WeightedTier(6f, 8f, 2),
                new TTFloat.WeightedTier(8f, 10f, 1.5),
                new TTFloat.WeightedTier(10f, 12f, 1),
                new TTFloat.WeightedTier(12f, 14f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Wounding", 0.5),
            new WeightedTierName("of Harming", 1),
            new WeightedTierName("of Laceration", 1.5),
            new WeightedTierName("of Hemmorrhage", 2),
            new WeightedTierName("of Suffering", 2.5),
            new WeightedTierName("of Agony", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(), Type2.GetMinValueFormat(), Type2.GetMaxValueFormat(), useChatTags);
            var valueRange3 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type3.GetCurrentValueFormat(1), Type3.GetMinValueFormat(1), Type3.GetMaxValueFormat(1), useChatTags);
            return $"{ valueRange1 }% chance to { Keyword.GetTextOrTag(KeywordType.Bleed, useChatTags) }({ valueRange2 }%) for { valueRange3 }s";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            OnHit(item, player, target, damage);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            OnHit(item, player, target, damage);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            OnHit(item, player, target, damage);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            OnHit(item, player, target, damage);
        }

        void OnHit(Item item, Player player, NPC target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
                Bleed(target, hitDamage);
        }
        void OnHit(Item item, Player player, Player target, int hitDamage)
        {
            if (item == player.HeldItem && Main.rand.NextFloat() < Type1.GetValue())
                Bleed(target, hitDamage);
        }

        void Bleed(NPC target, int hitDamage)
        {
            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, int.MaxValue);
            int duration = (int)MathHelper.Clamp(Type3.GetValue() * 60, 1, int.MaxValue);
            BuffNPC pomNPC = target.GetGlobalNPC<BuffNPC>();
            pomNPC.AddBleedBuff(target, damage, duration);
        }
        void Bleed(Player target, int hitDamage)
        {
            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, int.MaxValue);
            int duration = (int)MathHelper.Clamp(Type3.GetValue() * 60, 1, int.MaxValue);
            target.GetModPlayer<BuffPlayer>().AddBleedBuff(target, damage, duration);
        }
    }
}