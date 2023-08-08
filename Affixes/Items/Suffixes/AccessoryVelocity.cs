﻿using Microsoft.Xna.Framework;
using Terraria;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class AccessoryVelocity : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-10, -6, 0.5),
                new TTFloat.WeightedTier(-6, -3, 1),
                new TTFloat.WeightedTier(-3, 0, 2),
                new TTFloat.WeightedTier(0, 3, 2),
                new TTFloat.WeightedTier(3, 6, 1),
                new TTFloat.WeightedTier(6, 10, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Repulsion", 3),
            new WeightedTierName("of Repel", 2),
            new WeightedTierName("of Fending", 0.5),
            new WeightedTierName("of Pulling", 0.5),
            new WeightedTierName("of Attraction", 2),
            new WeightedTierName("of Magnetism", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsAccessory(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(1), Type1.GetMinValueFormat(1), Type1.GetMaxValueFormat(1), useChatTags);
            string towardsAway = Type1.GetValue() >= 0 ? "towards" : "away from";
            return $"Gain { valueRange1 } velocity { towardsAway } target when hit";
        }

        public override void OnHitByNPC(Item item, Player player, NPC npc, Player.HurtInfo hurtInfo)
        {
            GainVelocity(item, player, npc.Center);
        }
        public override void OnHitByPvp(Item item, Player player, Player attacker, Player.HurtInfo info)
        {
            GainVelocity(item, player, attacker.Center);
        }
        public override void OnHitByProjectile(Item item, Player player, Projectile projectile, Player.HurtInfo hurtInfo)
        {
            GainVelocity(item, player, projectile.Center);
        }

        void GainVelocity(Item item, Player player, Vector2 position)
        {
            if (ItemItem.IsAccessoryEquipped(item, player))
            {
                Vector2 addVelocity = (position - player.Center).SafeNormalize(Vector2.Zero) * Type1.GetValue();
                player.velocity += addVelocity;
            }
        }
    }
}