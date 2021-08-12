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
    public class WeaponCrit : AffixTiered<TTFloat, TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(-0.875f, -0.833f, 0.5),
                new TTFloat.WeightedTier(-0.833f, -0.750f, 1),
                new TTFloat.WeightedTier(-0.750f, -0.100f, 2),
                new TTFloat.WeightedTier(1f, 3f, 2),
                new TTFloat.WeightedTier(3f, 5f, 1),
                new TTFloat.WeightedTier(5f, 7f, 0.5),
            },
        };
        public override TTFloat Type2 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(5f, 4.5f, 0.5),
                new TTFloat.WeightedTier(4.5f, 4f, 1),
                new TTFloat.WeightedTier(4f, 3.5f, 2),
                new TTFloat.WeightedTier(3.5f, 3f, 2),
                new TTFloat.WeightedTier(3f, 2.5f, 1),
                new TTFloat.WeightedTier(2.5f, 2f, 0.5),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Debilitation", 3),
            new WeightedTierName("of Fatigue", 2),
            new WeightedTierName("of Stumbling", 0.5),
            new WeightedTierName("of Anticipation", 0.5),
            new WeightedTierName("of Preparation", 2),
            new WeightedTierName("of Planning", 3),
        };

        public uint lastProcTime = 0;

        public override bool CanRoll(ItemItem pomItem, Item item)
        {
            return
                ItemItem.IsWeapon(item);
        }

        public override string GetTolltipText()
        {
            string plusMinus = Type1.GetValue() >= 0 ? "+" : "-";

            return $"Deal {plusMinus}{Type1.GetValueFormat()}% damage ({Type2.GetValueFormat(1)}s CD)";
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit)
        {
            Hit(item, player, target, ref damageMultiplier);
        }
        public override void ModifyHitPvp(Item item, Player player, Player target, ref float damageMultiplier, ref bool crit)
        {
            Hit(item, player, target, ref damageMultiplier);
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit, ref int hitDirection)
        {
            Hit(item, player, target, ref damageMultiplier);
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref bool crit)
        {
            Hit(item, player, target, ref damageMultiplier);
        }

        void Hit(Item item, Player player, NPC target, ref float damageMultiplier)
        {
            if (item == player.HeldItem && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type2.GetValue() * 60))
            {
                Crit(target, ref damageMultiplier);
            }
        }
        void Hit(Item item, Player player, Player target, ref float damageMultiplier)
        {
            if (item == player.HeldItem && (Main.GameUpdateCount - lastProcTime) >= (int)Math.Round(Type2.GetValue() * 60))
            {
                Crit(target, ref damageMultiplier);
            }
        }

        void Crit(NPC target, ref float damageMultiplier)
        {
            damageMultiplier += Type1.GetValue();
            PoMEffectHelper.Crit(target.position, target.width, target.height, 50);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                EffectPacketHandler.CSyncCrit(target, 50);
            }
            lastProcTime = Main.GameUpdateCount;
        }
        void Crit(Player target, ref float damageMultiplier)
        {
            damageMultiplier += Type1.GetValue();
            PoMEffectHelper.Crit(target.position, target.width, target.height, 50);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                EffectPacketHandler.CSyncCrit(target, 50);
            }
            lastProcTime = Main.GameUpdateCount;
        }

        public override Affix Clone()
        {
            var affix = (WeaponCrit)base.Clone();

            affix.lastProcTime = lastProcTime;

            return affix;
        }
    }
}