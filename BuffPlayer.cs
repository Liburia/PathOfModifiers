using IL.Terraria.Achievements;
using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.Buffs;
using PathOfModifiers.ModNet.PacketHandlers;
using PathOfModifiers.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Net;

namespace PathOfModifiers
{
    public class BuffPlayer : ModPlayer
    {
        public bool isOnBurningAir;
        public bool isIgnited;
        public bool isOnShockedAir;
        public bool isShocked;
        public bool isOnChilledAir;
        public bool isChilled;

        int burningAirDps;
        int igniteDps;
        float shockedAirMultiplier;
        float shockedMultiplier;
        float chilledAirMultiplier;
        float chilledMultiplier;

        DoTInstanceCollection dotInstanceCollection = new DoTInstanceCollection();
        TimedValueInstanceCollection timedValueInstanceCollection = new TimedValueInstanceCollection();

        float moveSpeedBuffMultiplier = 1;
        public bool moveSpeedBuff = false;

        int staticStrikeDamage;
        int staticStrikeIntervalTicks;
        int staticStrikeCurrentInterval;
        public bool staticStrikeBuff = false;

        public override void Initialize()
        {
            dotInstanceCollection = new DoTInstanceCollection();
            timedValueInstanceCollection = new TimedValueInstanceCollection();
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            ShockModifyDamageTaken(ref damage);
            return true;
        }
        public override void ResetEffects()
        {
            isOnBurningAir = false;
            isIgnited = false;
            isOnShockedAir = false;
            isShocked = false;
            isOnChilledAir = false;
            isChilled = false;

            dotInstanceCollection.ResetEffects();
            timedValueInstanceCollection.ResetEffects();

            moveSpeedBuff = false;

            staticStrikeBuff = false;
        }
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            ChillModifyDamageDealt(ref damage);
        }
        public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
        {
            target.GetModPlayer<BuffPlayer>().ShockModifyDamageTaken(ref damage);
            ChillModifyDamageDealt(ref damage);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            ChillModifyDamageDealt(ref damage);
        }
        public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
        {
            target.GetModPlayer<BuffPlayer>().ShockModifyDamageTaken(ref damage);
            ChillModifyDamageDealt(ref damage);
        }
        public override void PostUpdateRunSpeeds()
        {
            if (moveSpeedBuff)
            {
                player.runAcceleration *= moveSpeedBuffMultiplier;
                player.maxRunSpeed *= moveSpeedBuffMultiplier;
                player.accRunSpeed *= moveSpeedBuffMultiplier;
            }
        }
        public override void PostUpdateEquips()
        {
            var affixPlayer = player.GetModPlayer<AffixItemPlayer>();
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.DodgeChance), out var dodgeChances))
            {
                foreach (var dodgeChance in dodgeChances.instances)
                {
                    affixPlayer.dodgeChance += dodgeChance.value;
                }
            }
        }
        public override void UpdateBadLifeRegen()
        {
            foreach (var kv in dotInstanceCollection.dotInstances)
            {
                Type type = kv.Key;
                int dps = kv.Value.dps;
                if (dps > 0)
                {
                    int debuffDamage = (int)Math.Round(dps * DamageOverTime.damageMultiplierHalfSecond);
                    player.lifeRegenTime = 0;
                    if (player.lifeRegen > 0)
                    {
                        player.lifeRegen = 0;
                    }
                    player.lifeRegen -= debuffDamage;

                    //TODO: this only works with buffs from this mod; could use BuffLoader.buffs
                    player.AddBuff(mod.BuffType(type.Name), 2, true);
                }
            }
            if (isOnBurningAir && burningAirDps > 0)
            {
                int debuffDamage = (int)Math.Round(burningAirDps * DamageOverTime.damageMultiplierHalfSecond);
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegen -= debuffDamage;
            }
            if (isIgnited && igniteDps > 0)
            {
                int debuffDamage = (int)Math.Round(igniteDps * DamageOverTime.damageMultiplierHalfSecond);
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegen -= debuffDamage;
            }
        }
        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
            if (removeDebuffs)
            {
                dotInstanceCollection.Clear();
            }
        }
        public override void PreUpdate()
        {
            if (PathOfModifiers.Time % 60 == 0)
            {
                if (isOnBurningAir && burningAirDps < 0)
                {
                    int heal = Math.Abs(burningAirDps);
                    player.statLife += heal;
                    player.HealEffect(heal);
                }
                if (isIgnited && igniteDps < 0)
                {
                    int heal = Math.Abs(igniteDps);
                    player.statLife += heal;
                    player.HealEffect(heal);
                }
            }

            if (staticStrikeBuff)
            {
                staticStrikeCurrentInterval++;

                if (staticStrikeCurrentInterval >= staticStrikeIntervalTicks)
                {
                    Projectile.NewProjectile(
                        position: player.Center,
                        velocity: Vector2.Zero,
                        Type: ModContent.ProjectileType<Projectiles.StaticStrike>(),
                        Damage: staticStrikeDamage,
                        KnockBack: 0,
                        Owner: player.whoAmI);

                    staticStrikeCurrentInterval = 0;
                }
            }
        }

        public void AddDoTBuff(Player player, DamageOverTime buff, int dps, int durationTicks, bool syncMP = true)
        {
            Type dotBuffType = buff.GetType();
            double durationMs = (durationTicks / 60f) * 1000;
            dotInstanceCollection.AddInstance(dotBuffType, dps, durationMs);

            if (syncMP && Main.netMode == NetmodeID.MultiplayerClient)
            {
                BuffPacketHandler.CSendAddDoTBuffPlayer(player.whoAmI, buff.Type, dps, durationTicks);
            }
        }
        public void AddMoveSpeedBuff(Player player, float speedMultiplier, int time, bool syncMP = true)
        {
            moveSpeedBuffMultiplier = speedMultiplier;
            player.AddBuff(ModContent.BuffType<MoveSpeed>(), time, true);

            if (Main.netMode != NetmodeID.SinglePlayer && syncMP)
            {
                BuffPacketHandler.CSendAddMoveSpeedBuffPlayer(player.whoAmI, speedMultiplier, time);
            }
        }
        public void AddBurningAirBuff(Player player, int dps)
        {
            burningAirDps = dps;
            player.AddBuff(ModContent.BuffType<BurningAir>(), 2, true);
        }
        public void AddIgnitedBuff(Player player, int dps, int durationTicks, bool syncMP = true)
        {
            igniteDps = dps;
            player.AddBuff(ModContent.BuffType<Ignited>(), durationTicks, true);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.CSendAddIgnitedBuffPlayer(player.whoAmI, dps, durationTicks);
            }
        }
        public void AddShockedAirBuff(Player player, float multiplier)
        {
            shockedAirMultiplier = multiplier;
            player.AddBuff(ModContent.BuffType<ShockedAir>(), 2, true);
        }
        public void AddShockedBuff(Player player, float multiplier, int durationTicks, bool syncMP = true)
        {
            shockedMultiplier = multiplier;
            player.AddBuff(ModContent.BuffType<Shocked>(), durationTicks, true);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.CSendAddShockedBuffPlayer(player.whoAmI, multiplier, durationTicks);
            }
        }
        public void AddChilledBuff(Player player, float multiplier, int durationTicks, bool syncMP = true)
        {
            chilledMultiplier = multiplier;
            player.AddBuff(ModContent.BuffType<Chilled>(), durationTicks, true);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.CSendAddChilledBuffPlayer(player.whoAmI, multiplier, durationTicks);
            }
        }
        public void AddChilledAirBuff(Player player, float multiplier)
        {
            chilledAirMultiplier = multiplier;
            player.AddBuff(ModContent.BuffType<ChilledAir>(), 2, true);
        }
        public void AddStaticStrikeBuff(Player player, int damage, int intervalTicks, int time, bool syncMP = true)
        {
            if (!staticStrikeBuff)
            {
                staticStrikeCurrentInterval = 0;
            }
            staticStrikeDamage = damage;
            staticStrikeIntervalTicks = intervalTicks;
            player.AddBuff(ModContent.BuffType<StaticStrike>(), time, true);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.CSendAddStaticStrikeBuffPlayer(player.whoAmI, damage, intervalTicks, time);
            }
        }
        public void AddDodgeChanceBuff(Player player, float chance, int durationTicks, bool syncMP = true)
        {
            double durationMs = (durationTicks / 60f) * 1000;
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.DodgeChance), chance, durationMs);

            if (syncMP && Main.netMode == NetmodeID.MultiplayerClient)
            {
                BuffPacketHandler.CSendAddDodgeChanceBuffPlayer(player.whoAmI, chance, durationTicks);
            }
        }

        public void ShockModifyDamageTaken(ref int damage)
        {
            float totalMultiplier = 1;
            if (isOnShockedAir)
            {
                totalMultiplier += shockedAirMultiplier - 1;
            }
            if (isShocked)
            {
                totalMultiplier += shockedMultiplier - 1;
            }
            damage = (int)Math.Round(damage * totalMultiplier);
        }
        public void ChillModifyDamageDealt(ref int damage)
        {
            float totalMultiplier = 1;
            if (isOnChilledAir)
            {
                totalMultiplier += chilledAirMultiplier - 1;
            }
            if (isChilled)
            {
                totalMultiplier += chilledMultiplier - 1;
            }
            damage = (int)Math.Round(damage * totalMultiplier);
        }
    }
}