using Microsoft.Xna.Framework;
using System;
using Terraria;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponMoveSpeed : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
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
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(1.0f, 1.5f, 3),
                new TTFloat.WeightedTier(1.5f, 2.0f, 2.5),
                new TTFloat.WeightedTier(2.0f, 2.5f, 2),
                new TTFloat.WeightedTier(2.5f, 3.0f, 1.5),
                new TTFloat.WeightedTier(3.0f, 3.5f, 1),
                new TTFloat.WeightedTier(3.5f, 4.0f, 0.5),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(6f, 5f, 3),
                new TTFloat.WeightedTier(5f, 4f, 2.5),
                new TTFloat.WeightedTier(4f, 3f, 2),
                new TTFloat.WeightedTier(3f, 2f, 1.5),
                new TTFloat.WeightedTier(2f, 1f, 1),
                new TTFloat.WeightedTier(1f, 0f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Spurring", 0.5),
            new WeightedTierName("of Hurry", 1),
            new WeightedTierName("of Quickening", 1.5),
            new WeightedTierName("of Haste", 2),
            new WeightedTierName("of Stimulation", 2.5),
            new WeightedTierName("of Rush", 3),
        };

        public uint lastProcTime = 0;

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(1), Type2.GetMinValueFormat(1), Type2.GetMaxValueFormat(1), useChatTags);
            var valueRange3 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type3.GetCurrentValueFormat(1), Type3.GetMinValueFormat(1), Type3.GetMaxValueFormat(1), useChatTags);
            string plusMinus = Type3.GetValue() >= 0 ? "+" : "-";
            return $"Gain { plusMinus }{ valueRange1 }% move speed on hit for { valueRange2 }s ({ valueRange3 }s CD)";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHit(item, player);
        }
        public override void OnHitPvp(Item item, Player player, Player target, Player.HurtInfo hurtInfo)
        {
            OnHit(item, player);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHit(item, player);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, Player.HurtModifiers modifiers, int damageDone)
        {
            OnHit(item, player);
        }

        void OnHit(Item item, Player player)
        {
            if (item == player.HeldItem && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type3.GetValue() * 60))
                GainMoveSpeed(player);
        }

        void GainMoveSpeed(Player player)
        {
            int duration = (int)MathHelper.Clamp(Type2.GetValue() * 60, 1, 9999999);
            player.GetModPlayer<BuffPlayer>().AddWeaponMoveSpeedBuff(player, Type1.GetValue(), duration);
            lastProcTime = Main.GameUpdateCount;
        }

        public override Affix Clone()
        {
            var affix = (WeaponMoveSpeed)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}