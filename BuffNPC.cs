using PathOfModifiers.Buffs;
using PathOfModifiers.ModNet.PacketHandlers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers
{
    public class BuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        TimedValueInstanceCollection timedValueInstanceCollection = new TimedValueInstanceCollection();

        public void AddBleedBuff(NPC npc, int dps, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Bleed), dps, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.CSendAddBleedBuffNPC(npc.whoAmI, dps, durationTicks);
            }
        }
        public void AddPoisonBuff(NPC npc, int dps, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Poison), dps, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.CSendAddPoisonBuffNPC(npc.whoAmI, dps, durationTicks);
            }
        }
        public void AddBurningAirBuff(NPC npc, int dps)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.BurningAir), dps, 2);
        }
        public void AddIgnitedBuff(NPC npc, int dps, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Ignite), dps, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddIgnitedBuffNPC(npc.whoAmI, dps, durationTicks);
            }
        }
        public void AddShockedAirBuff(NPC npc, float multiplier)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.ShockedAir), multiplier, 2);
        }
        public void AddShockedBuff(NPC npc, float multiplier, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Shock), multiplier, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddShockedBuffNPC(npc.whoAmI, multiplier, durationTicks);
            }
        }
        public void AddChilledBuff(NPC npc, float multiplier, int durationTicks, bool syncMP = true)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.Chill), multiplier, durationTicks);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.SendAddChilledBuffNPC(npc.whoAmI, multiplier, durationTicks);
            }
        }
        public void AddChilledAirBuff(NPC npc, float multiplier)
        {
            timedValueInstanceCollection.AddInstance(typeof(TimedValueInstanceCollection.InstanceType.ChilledAir), multiplier, 2);
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

        public override void AI(NPC npc)
        {
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Bleed), out var bleeds))
            {
                if (bleeds.totalValue > 0)
                {
                    PoMEffectHelper.Bleed(npc.Center);
                }
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Poison), out var poisons))
            {
                if (poisons.totalValue > 0)
                {
                    PoMEffectHelper.Poison(npc.position, npc.width, npc.height);
                }
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Ignite), out var ignites))
            {
                if (ignites.totalValue > 0)
                {
                    PoMEffectHelper.Ignite(npc.position, npc.width, npc.height);
                }
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Shock), out var shocks))
            {
                if (shocks.totalValue > 0)
                {
                    PoMEffectHelper.Shock(npc.position, npc.width, npc.height);
                }
            }
            if (timedValueInstanceCollection.instances.TryGetValue(typeof(TimedValueInstanceCollection.InstanceType.Chill), out var chills))
            {
                if (chills.totalValue > 0)
                {
                    PoMEffectHelper.Chill(npc.position, npc.width, npc.height);
                }
            }
        }

        public StatModifier ShockModifyDamageTaken(StatModifier damage)
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

            return damage * totalMultiplier;
        }
        public StatModifier ChillModifyDamageDealt(StatModifier damage)
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

            return damage * totalMultiplier;
        }

        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            var damage = ShockModifyDamageTaken(modifiers.FinalDamage);
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            var damage = ShockModifyDamageTaken(modifiers.FinalDamage);
        }
        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            var damage = ChillModifyDamageDealt(modifiers.FinalDamage);
        }
        public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
        {
            var damage = ChillModifyDamageDealt(modifiers.FinalDamage);
        }
    }
}