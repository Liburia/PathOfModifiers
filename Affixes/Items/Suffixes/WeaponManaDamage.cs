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
    public class WeaponManaDamage : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0.3f, 3),
                new TTFloat.WeightedTier(0.27f, 2.5),
                new TTFloat.WeightedTier(0.23f, 2),
                new TTFloat.WeightedTier(0.2f, 1.5),
                new TTFloat.WeightedTier(0.17f, 1),
                new TTFloat.WeightedTier(0.13f, 0.5),
                new TTFloat.WeightedTier(0.1f, 0),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.1f, 2.5),
                new TTFloat.WeightedTier(0.2f, 2),
                new TTFloat.WeightedTier(0.3f, 1.5),
                new TTFloat.WeightedTier(0.4f, 1),
                new TTFloat.WeightedTier(0.5f, 0.5),
                new TTFloat.WeightedTier(0.6f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Intuition", 0.5),
            new WeightedTierName("of Insight", 1),
            new WeightedTierName("of Acumen", 1.5),
            new WeightedTierName("of Ascendance", 2),
            new WeightedTierName("of Transcendence", 2.5),
            new WeightedTierName("of Divinity", 3),
        };

        double lastProcTime = 0;

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            string plusMinus = Type2.GetValue() >= 1 ? "" : "-";

            return $"Spend {Type1.GetValueFormat()}% mana to increase damage by {plusMinus}{Type2.GetValueFormat()}%";
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit)
        {
            ModifyDamage(item, player, ref damageMultiplier);
        }
        public override void ModifyHitPvp(Item item, Player player, Player target, ref float damageMultiplier, ref bool crit)
        {
            ModifyDamage(item, player, ref damageMultiplier);
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit, ref int hitDirection)
        {
            ModifyDamage(item, player, ref damageMultiplier);
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref bool crit)
        {
            ModifyDamage(item, player, ref damageMultiplier);
        }

        void ModifyDamage(Item item, Player player, ref float damageMultiplier)
        {
            if (item == player.HeldItem && TryConsumeMana(player))
            {
                damageMultiplier += Type2.GetValue();
            }
        }

        bool TryConsumeMana(Player player)
        {
            int amount = (int)Math.Round(player.statManaMax2 * Type1.GetValue());
            return player.CheckMana(amount, true);
        }
    }
}