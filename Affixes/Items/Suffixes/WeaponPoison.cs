using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Collections.Generic;
using Terraria.ModLoader.IO;
using PathOfModifiers.Projectiles;
using PathOfModifiers.Buffs;
using PathOfModifiers.UI.Chat;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponPoison : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
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
                new TTFloat.WeightedTier(0.55f, 0.75f, 1.5),
                new TTFloat.WeightedTier(0.75f, 0.85f, 1),
                new TTFloat.WeightedTier(0.85f, 1.0f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.010f, 0.018f, 3),
                new TTFloat.WeightedTier(0.018f, 0.027f, 2.5),
                new TTFloat.WeightedTier(0.027f, 0.035f, 2),
                new TTFloat.WeightedTier(0.035f, 0.044f, 1.5),
                new TTFloat.WeightedTier(0.044f, 0.052f, 1),
                new TTFloat.WeightedTier(0.052f, 0.060f, 0.5),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(9f, 12f, 3),
                new TTFloat.WeightedTier(12f, 15f, 2.5),
                new TTFloat.WeightedTier(15f, 18f, 2),
                new TTFloat.WeightedTier(18f, 21f, 1.5),
                new TTFloat.WeightedTier(21f, 24f, 1),
                new TTFloat.WeightedTier(24f, 27f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Venom", 0.5),
            new WeightedTierName("of Toxicity", 1),
            new WeightedTierName("of Misery", 1.5),
            new WeightedTierName("of Virulence", 2),
            new WeightedTierName("of Blight", 2.5),
            new WeightedTierName("of Miasma", 3),
        };


        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetAffixText(bool useChatTags = false)
        {
            var valueRange1 = ValueRangeTagHandler.GetTextOrTag(Type1.GetCurrentValueFormat(), Type1.GetMinValueFormat(), Type1.GetMaxValueFormat(), useChatTags);
            var valueRange2 = ValueRangeTagHandler.GetTextOrTag(Type2.GetCurrentValueFormat(), Type2.GetMinValueFormat(), Type2.GetMaxValueFormat(), useChatTags);
            var valueRange3 = ValueRangeTagHandler.GetTextOrTag(Type3.GetCurrentValueFormat(1), Type3.GetMinValueFormat(1), Type3.GetMaxValueFormat(1), useChatTags);
            return $"{ valueRange1 }% chance to { Keyword.GetTextOrTag(KeywordType.Poison, useChatTags) }({ valueRange2 }%) for { valueRange3 }s";
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
            pomNPC.AddPoisonBuff(target, damage, duration);
        }
        void Bleed(Player target, int hitDamage)
        {
            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, int.MaxValue);
            int duration = (int)MathHelper.Clamp(Type3.GetValue() * 60, 1, int.MaxValue);
            target.GetModPlayer<BuffPlayer>().AddPoisonBuff(target, damage, duration);
        }
    }
}