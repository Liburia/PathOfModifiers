using IL.Terraria.Achievements;
using IL.Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.ModNet.PacketHandlers;
using PathOfModifiers.Projectiles;
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
        TimedValueInstanceCollection timedValueInstanceCollection;

        int staticStrikeDamage;
        int staticStrikeIntervalTicks;
        int staticStrikeCurrentInterval;
        public bool staticStrikeBuff = false;

        int moltenShellStoredDamage;
        int moltenShellTimeLeft;

        int noManaCostTimeLeft;
        int knockbackImmunityTimeLeft;

        public override void Initialize()
        {
            timedValueInstanceCollection = new TimedValueInstanceCollection();
        }

        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            damage = ShockModifyDamageTaken(damage);
            return true;
        }
        public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
        {
            if (moltenShellTimeLeft > 0)
            {
                moltenShellStoredDamage += (int)damage;
            }
        }
        public override void ResetEffects()
        {
            timedValueInstanceCollection.ResetEffects();

            staticStrikeBuff = false;
        }
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            damage = ChillModifyDamageDealt(damage);
        }
        public override void ModifyHitPvp(Item item, Player target, ref int damage, ref bool crit)
        {
            damage = ChillModifyDamageDealt(damage);
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = ChillModifyDamageDealt(damage);
        }
        public override void ModifyHitPvpWithProj(Projectile proj, Player target, ref int damage, ref bool crit)
        {
            damage = ChillModifyDamageDealt(damage);
        }
        public override void PostUpdateEquips()
        {
            var affixPlayer = player.GetModPlayer<AffixItemPlayer>();
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.DodgeChance), out var dodgeChances))
            {
                affixPlayer.dodgeChance += dodgeChances.totalValue;
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.MoveSpeed), out var moveSpeeds))
            {
                affixPlayer.moveSpeed += moveSpeeds.totalValue;
            }

            if (knockbackImmunityTimeLeft > 0)
            {
                player.noKnockback = true;
            }

            if (moltenShellTimeLeft > 0)
            {
                affixPlayer.damageTaken += -0.1f;
            }
        }
        public override void UpdateBadLifeRegen()
        {
            float totalDPS = 0;

            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Bleed), out var bleeds))
            {
                totalDPS += bleeds.totalValue;
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Poison), out var poisons))
            {
                totalDPS += poisons.totalValue;
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Ignite), out var ignites))
            {
                totalDPS += ignites.totalValue;
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.BurningAir), out var burningAirs))
            {
                totalDPS += burningAirs.totalValue;
            }


            if (totalDPS > 0)
            {
                int totalDamage = (int)Math.Round(totalDPS * Buffs.DamageOverTime.damageMultiplierHalfSecond);
                player.lifeRegenTime = 0;
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegen -= totalDamage;
            }
        }
        public override void PostNurseHeal(NPC nurse, int health, bool removeDebuffs, int price)
        {
            if (removeDebuffs)
            {
                timedValueInstanceCollection.RemoveInstances(typeof(TimedValueInstanceCollection.InstanceType.Bleed));
                timedValueInstanceCollection.RemoveInstances(typeof(TimedValueInstanceCollection.InstanceType.Poison));
                timedValueInstanceCollection.RemoveInstances(typeof(TimedValueInstanceCollection.InstanceType.Shock));
                timedValueInstanceCollection.RemoveInstances(typeof(TimedValueInstanceCollection.InstanceType.Ignite));
                timedValueInstanceCollection.RemoveInstances(typeof(TimedValueInstanceCollection.InstanceType.Chill));
            }
        }
        public override void PreUpdate()
        {
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

            if (noManaCostTimeLeft > 0)
            {
                noManaCostTimeLeft--;
            }
            if (knockbackImmunityTimeLeft > 0)
            {
                knockbackImmunityTimeLeft--;
            }
            if (moltenShellTimeLeft > 0)
            {
                moltenShellTimeLeft--;

                if (player.whoAmI == Main.myPlayer && moltenShellTimeLeft <= 0)
                {
                    Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<MoltenShellExplosion>(), moltenShellStoredDamage, 5, player.whoAmI, moltenShellStoredDamage / (float)player.statLifeMax2);
                }
            }
        }
        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            if (noManaCostTimeLeft > 0)
            {
                mult = -10f;
            }
        }

        public void AddBleedBuff(Player player, int dps, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Bleed), dps, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddBleedBuffPlayer(player.whoAmI, dps, durationTicks);
            }
        }
        public void AddPoisonBuff(Player player, int dps, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Poison), dps, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddPoisonBuffPlayer(player.whoAmI, dps, durationTicks);
            }
        }
        public void AddMoveSpeedBuff(Player player, float speedBoost, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.MoveSpeed), speedBoost, durationTicks);

            if (Main.netMode != NetmodeID.SinglePlayer && syncMP)
            {
                BuffPacketHandler.SendAddMoveSpeedBuffPlayer(player.whoAmI, speedBoost, durationTicks);
            }
        }
        public void AddBurningAirBuff(Player player, int dps)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.BurningAir), dps, 2);
        }
        public void AddIgnitedBuff(Player player, int dps, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Ignite), dps, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddIgnitedBuffPlayer(player.whoAmI, dps, durationTicks);
            }
        }
        public void AddShockedAirBuff(Player player, float multiplier)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.ShockedAir), multiplier, 2);
        }
        public void AddShockedBuff(Player player, float multiplier, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Shock), multiplier, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddShockedBuffPlayer(player.whoAmI, multiplier, durationTicks);
            }
        }
        public void AddChilledBuff(Player player, float multiplier, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Chill), multiplier, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddChilledBuffPlayer(player.whoAmI, multiplier, durationTicks);
            }
        }
        public void AddChilledAirBuff(Player player, float multiplier)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.ChilledAir), multiplier, 2);
        }
        public void AddStaticStrikeBuff(Player player, int damage, int intervalTicks, int time, bool syncMP = true)
        {
            if (!staticStrikeBuff)
            {
                staticStrikeCurrentInterval = 0;
            }
            staticStrikeDamage = damage;
            staticStrikeIntervalTicks = intervalTicks;
            player.AddBuff(ModContent.BuffType<Buffs.StaticStrike>(), time, true);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddStaticStrikeBuffPlayer(player.whoAmI, damage, intervalTicks, time);
            }
        }
        public void AddDodgeChanceBuff(Player player, float chance, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.DodgeChance), chance, durationTicks);

            if (syncMP && Main.netMode == NetmodeID.MultiplayerClient)
            {
                BuffPacketHandler.SendAddDodgeChanceBuffPlayer(player.whoAmI, chance, durationTicks);
            }
        }
        public void AddNoManaCostBuff(Player player, int durationTicks, bool syncMP = true)
        {
            if (durationTicks > noManaCostTimeLeft)
            {
                noManaCostTimeLeft = durationTicks;

                if (syncMP && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    BuffPacketHandler.SendAddNoManaCostBuffPlayer(player.whoAmI, durationTicks);
                }
            }

        }
        public void AddKnockbackImmunityBuff(Player player, int durationTicks, bool syncMP = true)
        {
            if (durationTicks > knockbackImmunityTimeLeft)
            {
                knockbackImmunityTimeLeft = durationTicks;

                if (syncMP && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    BuffPacketHandler.SendAddKnockbackImmunityBuffPlayer(player.whoAmI, durationTicks);
                }
            }
        }
        public void AddMoltenShellBuff(Player player, int durationTicks, bool syncMP = true)
        {
            if (durationTicks > moltenShellTimeLeft)
            {
                moltenShellTimeLeft = durationTicks;

                if (syncMP && Main.netMode == NetmodeID.MultiplayerClient)
                {
                    BuffPacketHandler.SendAddMoltenShellBuffPlayer(player.whoAmI, durationTicks);
                }
            }
        }

        public int ShockModifyDamageTaken(int damage)
        {
            float totalMultiplier = 1;

            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Shock), out var shocks))
            {
                totalMultiplier += shocks.totalValue;
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.ShockedAir), out var shockedAirs))
            {
                totalMultiplier += shockedAirs.totalValue;
            }

            return (int)Math.Round(damage * totalMultiplier);
        }
        public int ChillModifyDamageDealt(int damage)
        {
            float totalMultiplier = 1;

            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Chill), out var chills))
            {
                totalMultiplier += chills.totalValue;
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.ChilledAir), out var chilledAirs))
            {
                totalMultiplier += chilledAirs.totalValue;
            }

            return (int)Math.Round(damage * totalMultiplier);
        }
    }
}