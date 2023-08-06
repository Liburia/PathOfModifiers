using System;
using Terraria;
using Terraria.ID;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class ArmorAssistance : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(10f, 40f, 3),
                new TTFloat.WeightedTier(40f, 70f, 1.5),
                new TTFloat.WeightedTier(70f, 100f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Optimization", 0.5),
            new WeightedTierName("of Efficacy", 2),
            new WeightedTierName("of Productivity", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(1), Type1.GetMinValueFormat(1), Type1.GetMaxValueFormat(1), useChatTags);
            return $"Gain assistance buffs when hit for { valueRange1 }s";
        }

        public override void OnHitByNPC(Item item, Player player, NPC npc, Player.HurtInfo hurtInfo)
        {
            GainBuffs(item, player);
        }
        public override void OnHitByPvp(Item item, Player player, Player attacker, Player.HurtInfo info)
        {
            GainBuffs(item, player);
        }
        public override void OnHitByProjectile(Item item, Player player, Projectile projectile, Player.HurtInfo hurtInfo)
        {
            GainBuffs(item, player);
        }

        void GainBuffs(Item item, Player player)
        {
            if (ItemItem.IsArmorEquipped(item, player))
            {
                int durationTicks = (int)Math.Round(Type1.GetValue() * 60);
                player.AddBuff(BuffID.Builder, durationTicks);
                player.AddBuff(BuffID.Mining, durationTicks);
                player.AddBuff(BuffID.NightOwl, durationTicks);
                player.AddBuff(BuffID.Shine, durationTicks);
                player.AddBuff(BuffID.Spelunker, durationTicks);
                player.AddBuff(BuffID.Heartreach, durationTicks);
            }
        }
    }
}