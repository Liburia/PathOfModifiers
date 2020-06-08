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
using PathOfModifiers.ModNet.PacketHandlers;
using Terraria.ID;

namespace PathOfModifiers.Affixes.Items.Suffixes
{
    public class WeaponFullHPCrit : AffixTiered<TTFloat>, ISuffix
    {
        public override double Weight { get; } = 1;

        public override TTFloat Type1 { get; } = new TTFloat()
        {
            TwoWay = false,
            IsRange = true,
            Tiers = new TTFloat.WeightedTier[]
            {
                new TTFloat.WeightedTier(0f, 3),
                new TTFloat.WeightedTier(0.016f, 2.5),
                new TTFloat.WeightedTier(0.033f, 2),
                new TTFloat.WeightedTier(0.05f, 1.5),
                new TTFloat.WeightedTier(0.066f, 1),
                new TTFloat.WeightedTier(0.084f, 0.5),
                new TTFloat.WeightedTier(0.1f, 0),
            },
        };
        public override WeightedTierName[] TierNames { get; } = new WeightedTierName[] {
            new WeightedTierName("of Decimation", 0.5),
            new WeightedTierName("of Butchery", 1),
            new WeightedTierName("of Slaying", 1.5),
            new WeightedTierName("of Assassination", 2),
            new WeightedTierName("of Eradication", 2.5),
            new WeightedTierName("of Annihilation", 3),
        };

        public override bool CanBeRolled(AffixItemItem pomItem, Item item)
        {
            return
                AffixItemItem.IsWeapon(item);
        }

        public override string GetTolltipText(Item item)
        {
            return $"Deal {Type1.GetValueFormat()}% of enemy HP with the first attack";
        }

        public override void ModifyHitNPC(Item item, Player player, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit)
        {
            Hit(item, player, target);
        }
        public override void ModifyHitPvp(Item item, Player player, Player target, ref float damageMultiplier, ref bool crit)
        {
            Hit(item, player, target);
        }
        public override void ProjModifyHitNPC(Item item, Player player, Projectile projectile, NPC target, ref float damageMultiplier, ref float knockbackMultiplier, ref bool crit, ref int hitDirection)
        {
            Hit(item, player, target);
        }
        public override void ProjModifyHitPvp(Item item, Player player, Projectile projectile, Player target, ref float damageMultiplier, ref bool crit)
        {
            Hit(item, player, target);
        }

        void Hit(Item item, Player player, NPC target)
        {
            //TODO: test worms and shit
            NPC realTarget = target.realLife >= 0 ? Main.npc[target.realLife] : target;
            if (item == player.HeldItem && realTarget.life >= realTarget.lifeMax)
            {
                DoDamage(player, target);
            }
        }
        void Hit(Item item, Player player, Player target)
        {
            if (item == player.HeldItem && target.statLife >= target.statLifeMax2)
            {
                DoDamage(player, target);
            }
        }

        void DoDamage(Player player, NPC target)
        {
            int critDamage = (int)Math.Round(target.lifeMax * Type1.GetValue());
            int direction = (target.Center.X - player.Center.X) > 0 ? 1 : -1;
            player.ApplyDamageToNPC(target, critDamage, 0, direction, false);
            PoMEffectHelper.Crit(target.position, target.width, target.height, 100);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                EffectPacketHandler.CSyncCrit(target, 100);
            }
        }
        void DoDamage(Player player, Player target)
        {
            int critDamage = (int)Math.Round(target.statLifeMax2 * Type1.GetValue());
            int direction = (target.Center.X - player.Center.X) > 0 ? 1 : -1;
            target.Hurt(Terraria.DataStructures.PlayerDeathReason.ByPlayer(player.whoAmI), critDamage, direction, true, false, false);
            PoMEffectHelper.Crit(target.position, target.width, target.height, 100);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                EffectPacketHandler.CSyncCrit(target, 100);
            }
        }
    }
}