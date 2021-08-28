using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using PathOfModifiers.Projectiles;
using Terraria.ID;
using PathOfModifiers.ModNet.PacketHandlers;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponConfusion : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.1f, 0.2f, 3),
                new TTFloat.WeightedTier(0.2f, 0.3f, 1.5),
                new TTFloat.WeightedTier(0.3f, 0.4f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(1f, 3f, 3),
                new TTFloat.WeightedTier(3f, 5f, 1.5),
                new TTFloat.WeightedTier(5f, 7f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Distraction", 0.5),
            new WeightedTierName("of Confusion", 2),
            new WeightedTierName("of Deception", 3),
        };

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = UI.Chat.ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(1), Type2.GetMinValueFormat(1), Type2.GetMaxValueFormat(1), useChatTags);
            return $"{ valueRange1 }% chance to confuse enemy for { valueRange2 }s";
        }

        public override void OnHitNPC(Item item, Player player, NPC target, int damage, float knockBack, bool crit)
        {
            Hit(item, player, target);
        }
        public override void OnHitPvp(Item item, Player player, Player target, int damage, bool crit)
        {
            Hit(item, player, target);
        }
        public override void ProjOnHitNPC(Item item, Player player, Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
            Hit(item, player, target);
        }
        public override void ProjOnHitPvp(Item item, Player player, Projectile projectile, Player target, int damage, bool crit)
        {
            Hit(item, player, target);
        }

        void Hit(Item item, Player player, NPC target)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(1) < Type1.GetValue())
            {
                int durationTicks = (int)Math.Round(Type2.GetValue() * 60);
                target.AddBuff(BuffID.Confused, durationTicks);
            }
        }
        void Hit(Item item, Player player, Player target)
        {
            if (item == player.HeldItem && Main.rand.NextFloat(1) < Type1.GetValue())
            {
                int durationTicks = (int)Math.Round(Type2.GetValue() * 60);
                target.AddBuff(BuffID.Confused, durationTicks, false);
            }
        }
    }
}