using System;
using Terraria;
using Terraria.ID;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class ArmorSafety : AffixTiered<TTFloat>, ISuffix
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
            new WeightedTierName("of Protection", 0.5),
            new WeightedTierName("of Safeguarding", 2),
            new WeightedTierName("of Bulwark", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAnyArmor(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(1), Type1.GetMinValueFormat(1), Type1.GetMaxValueFormat(1), useChatTags);
            return $"Gain safety buffs when hit for { valueRange1 }s";
        }

        public override void OnHitByNPC(Item item, Player player, NPC npc, int damage, bool crit)
        {
            GainBuffs(item, player);
        }
        public override void OnHitByPvp(Item item, Player player, Player attacker, int damage, bool crit)
        {
            GainBuffs(item, player);
        }
        public override void OnHitByProjectile(Item item, Player player, Projectile projectile, int damage, bool crit)
        {
            GainBuffs(item, player);
        }

        void GainBuffs(Item item, Player player)
        {
            if (ItemItem.IsArmorEquipped(item, player))
            {
                int durationTicks = (int)Math.Round(Type1.GetValue() * 60);
                player.AddBuff(BuffID.Dangersense, durationTicks);
                player.AddBuff(BuffID.Featherfall, durationTicks);
                player.AddBuff(BuffID.Gravitation, durationTicks);
                player.AddBuff(BuffID.Hunter, durationTicks);
                player.AddBuff(BuffID.ObsidianSkin, durationTicks);
            }
        }
    }
}