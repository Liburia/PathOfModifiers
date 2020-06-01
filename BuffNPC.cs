using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes.NPCs;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System;
using PathOfModifiers.Buffs;
using Terraria.ID;
using PathOfModifiers.Rarities;
using System.Text;
using System.IO;
using PathOfModifiers.ModNet.PacketHandlers;

namespace PathOfModifiers
{
    public class BuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        TimedValueInstanceCollection timedValueInstanceCollection = new TimedValueInstanceCollection();

        public void AddBleedBuff(NPC npc, int dps, int durationTicks, bool syncMP = true)
        {
            double durationMs = (durationTicks / 60f) * 1000;
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Bleed), dps, durationMs);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.CSendAddBleedBuffNPC(npc.whoAmI, dps, durationTicks);
            }
        }
        public void AddPoisonBuff(NPC npc, int dps, int durationTicks, bool syncMP = true)
        {
            double durationMs = (durationTicks / 60f) * 1000;
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Poison), dps, durationMs);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.CSendAddPoisonBuffNPC(npc.whoAmI, dps, durationTicks);
            }
        }
        public void AddBurningAirBuff(NPC npc, int dps)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.BurningAir), dps, PathOfModifiers.tickMS);
        }
        public void AddIgnitedBuff(NPC npc, int dps, int durationTicks, bool syncMP = true)
        {
            double durationMs = (durationTicks / 60f) * 1000;
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Ignite), dps, durationMs);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddIgnitedBuffNPC(npc.whoAmI, dps, durationTicks);
            }
        }
        public void AddShockedAirBuff(NPC npc, float multiplier)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.ShockedAir), multiplier, PathOfModifiers.tickMS);
        }
        public void AddShockedBuff(NPC npc, float multiplier, int durationTicks, bool syncMP = true)
        {
            double durationMs = (durationTicks / 60f) * 1000;
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Shock), multiplier, durationMs);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddShockedBuffNPC(npc.whoAmI, multiplier, durationTicks);
            }
        }
        public void AddChilledBuff(NPC npc, float multiplier, int durationTicks, bool syncMP = true)
        {
            double durationMs = (durationTicks / 60f) * 1000;
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Chill), multiplier, durationMs);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddChilledBuffNPC(npc.whoAmI, multiplier, durationTicks);
            }
        }
        public void AddChilledAirBuff(NPC npc, float multiplier)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.ChilledAir), multiplier, PathOfModifiers.tickMS);
        }

        public override void ResetEffects(NPC npc)
        {
            timedValueInstanceCollection.ResetEffects();
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
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
                int totalDamage = (int)Math.Round(totalDPS * DamageOverTime.damageMultiplierHalfSecond);
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= totalDamage;
            }

            if (npc.lifeRegen < 0)
            {
                damage = npc.lifeRegen / -4;
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

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            damage = ShockModifyDamageTaken(damage);
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage = ShockModifyDamageTaken(damage);
        }
        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            damage = ChillModifyDamageDealt(damage);
        }
        public override void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            damage = ChillModifyDamageDealt(damage);
        }
    }
}