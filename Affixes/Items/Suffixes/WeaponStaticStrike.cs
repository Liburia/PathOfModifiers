using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponStaticStrike : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(3f, 6f, 3),
                new TTFloat.WeightedTier(6f, 9f, 2.5),
                new TTFloat.WeightedTier(9f, 12f, 2),
                new TTFloat.WeightedTier(12f, 15f, 1.5),
                new TTFloat.WeightedTier(15f, 18f, 1),
                new TTFloat.WeightedTier(18f, 21f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.10f, 0.18f, 3),
                new TTFloat.WeightedTier(0.18f, 0.27f, 2.5),
                new TTFloat.WeightedTier(0.27f, 0.35f, 2),
                new TTFloat.WeightedTier(0.35f, 0.44f, 1.5),
                new TTFloat.WeightedTier(0.44f, 0.52f, 1),
                new TTFloat.WeightedTier(0.52f, 0.60f, 0.5),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = true,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(1f, 0.87f, 3),
                new TTFloat.WeightedTier(0.87f, 0.73f, 2.5),
                new TTFloat.WeightedTier(0.73f, 0.6f, 2),
                new TTFloat.WeightedTier(0.6f, 0.47f, 1.5),
                new TTFloat.WeightedTier(0.47f, 0.33f, 1),
                new TTFloat.WeightedTier(0.33f, 0.2f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Tantrum", 0.5),
            new WeightedTierName("of Irritation", 1),
            new WeightedTierName("of Temper", 1.5),
            new WeightedTierName("of Ire", 2),
            new WeightedTierName("of Storm", 2.5),
            new WeightedTierName("of Wrath", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(1), Type1.GetMinValueFormat(1), Type1.GetMaxValueFormat(1), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(), Type2.GetMinValueFormat(), Type2.GetMaxValueFormat(), useChatTags);
            var valueRange3 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type3.GetCurrentValueFormat(1), Type3.GetMinValueFormat(1), Type3.GetMaxValueFormat(1), useChatTags);
            return $"On hit gain a buff for { valueRange1 }s that does { valueRange2 }% damage every { valueRange3 }s";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Hit(item, player, damageDone);
        }
        public override void OnHitPvp(Item item, Player player, Player target, Player.HurtInfo hurtInfo)
        {
            Hit(item, player, hurtInfo.Damage);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Hit(item, player, damageDone);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, Player.HurtModifiers modifiers, int damageDone)
        {
            Hit(item, player, damageDone);
        }

        void Hit(Item item, Player player, int hitDamage)
        {
            if (item == player.HeldItem)
            {
                int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, 999999);

                int intervalTicks = (int)Math.Round(Type3.GetValue() * 60);
                if (intervalTicks < 1)
                {
                    intervalTicks = 1;
                }

                int durationTicks = (int)Math.Round(Type1.GetValue() * 60);
                if (durationTicks < 1)
                {
                    durationTicks = 1;
                }

                player.GetModPlayer<BuffPlayer>().AddStaticStrikeBuff(player, damage, intervalTicks, durationTicks, true);
            }
        }
    }
}