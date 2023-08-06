using System;
using Terraria;
using Terraria.ID;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponLiquid : AffixTiered<TTFloat>, ISuffix
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
            new WeightedTierName("of River", 0),
            new WeightedTierName("of Sea", 2),
            new WeightedTierName("of Ocean", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(1), Type1.GetMinValueFormat(1), Type1.GetMaxValueFormat(1), useChatTags);
            return $"Gain liquid buffs on hit for { valueRange1 }s";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Hit(item, player);
        }
        public override void OnHitPvp(Item item, Player player, Player target, Player.HurtInfo hurtInfo)
        {
            Hit(item, player);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Hit(item, player);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, Player.HurtModifiers modifiers, int damageDone)
        {
            Hit(item, player);
        }

        void Hit(Item item, Player player)
        {
            if (player.HeldItem == item)
            {
                int durationTicks = (int)Math.Round(Type1.GetValue() * 60);
                player.AddBuff(BuffID.Flipper, durationTicks);
                player.AddBuff(BuffID.Gills, durationTicks);
                player.AddBuff(BuffID.WaterWalking, durationTicks);
            }
        }
    }
}
