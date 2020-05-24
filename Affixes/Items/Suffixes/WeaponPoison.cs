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

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponPoison : AffixTiered<TTFloat, TTFloat, TTFloat>, ISuffix
    {
        public override double Weight => 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.16f, 2.5),
                new TTFloat.WeightedTier(0.33f, 2),
                new TTFloat.WeightedTier(0.5f, 1.5),
                new TTFloat.WeightedTier(0.66f, 1),
                new TTFloat.WeightedTier(0.84f, 0.5),
                new TTFloat.WeightedTier(1f, 0),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.01f, 2.5),
                new TTFloat.WeightedTier(0.02f, 2),
                new TTFloat.WeightedTier(0.03f, 1.5),
                new TTFloat.WeightedTier(0.04f, 1),
                new TTFloat.WeightedTier(0.05f, 0.5),
                new TTFloat.WeightedTier(0.06f, 0),
            },
        };
        public override TTFloat Type3 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(9f, 3),
                new TTFloat.WeightedTier(12f, 2.5),
                new TTFloat.WeightedTier(15f, 2),
                new TTFloat.WeightedTier(18f, 1.5),
                new TTFloat.WeightedTier(21f, 1),
                new TTFloat.WeightedTier(24f, 0.5),
                new TTFloat.WeightedTier(27f, 0),
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


        public override bool CanBeRolled(PoMItem pomItem, Item item)
        {
            return
                PoMItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"{Type1.GetValueFormat()}% chance to Poison({Type2.GetValueFormat()}%) for {Type3.GetValueFormat(1)}s";
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
            PoMNPC pomNPC = target.GetGlobalNPC<PoMNPC>();
            pomNPC.AddDoTBuff(target, ModContent.GetInstance<Poisoned>(), damage, duration);
        }
        void Bleed(Player target, int hitDamage)
        {
            int damage = (int)MathHelper.Clamp(hitDamage * Type2.GetValue(), 1, int.MaxValue);
            int duration = (int)MathHelper.Clamp(Type3.GetValue() * 60, 1, int.MaxValue);
            PoMPlayer pomPlayer = target.GetModPlayer<PoMPlayer>();
            pomPlayer.AddDoTBuff(target, ModContent.GetInstance<Poisoned>(), damage, duration);
        }
    }
}