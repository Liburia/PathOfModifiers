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
    public class PoMNPC : GlobalNPC
    {
        /// <summary>
        /// Set to true to skip rolling rarity and affixes for the next NPC that is spawned(and has its SetDefaults called)
        /// </summary>
        public static bool dontRollNextNPC = false;

        public override bool InstancePerEntity => true;

        public Entity lastDamageDealer;

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


        public void AddDoTBuff(NPC npc, DamageOverTime buff, int dps, int durationTicks, bool syncMP = true)
        {
            Type dotBuffType = buff.GetType();
            double durationMs = (durationTicks / 60f) * 1000;
            dotInstanceCollection.AddInstance(dotBuffType, dps, durationMs);

            if (syncMP && Main.netMode == NetmodeID.MultiplayerClient)
            {
                BuffPacketHandler.CSendAddDoTBuffNPC(npc.whoAmI, buff.Type, dps, durationTicks);
            }
        }
        public void AddBurningAirBuff(NPC npc, int dps)
        {
            burningAirDps = dps;
            npc.AddBuff(ModContent.BuffType<BurningAir>(), 2, true);
        }
        public void AddIgnitedBuff(NPC npc, int dps, int durationTicks, bool syncMP = true)
        {
            igniteDps = dps;
            npc.AddBuff(ModContent.BuffType<Ignited>(), durationTicks, true);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.CSendAddIgnitedBuffNPC(npc.whoAmI, dps, durationTicks);
            }
        }
        public void AddShockedAirBuff(NPC npc, float multiplier)
        {
            shockedAirMultiplier = multiplier;
            npc.AddBuff(ModContent.BuffType<ShockedAir>(), 2, true);
        }
        public void AddShockedBuff(NPC npc, float multiplier, int durationTicks, bool syncMP = true)
        {
            shockedMultiplier = multiplier;
            npc.AddBuff(ModContent.BuffType<Shocked>(), durationTicks, true);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.CSendAddShockedBuffNPC(npc.whoAmI, multiplier, durationTicks);
            }
        }
        public void AddChilledAirBuff(NPC npc, float multiplier)
        {
            chilledAirMultiplier = multiplier;
            npc.AddBuff(ModContent.BuffType<ChilledAir>(), 2, true);
        }
        public void AddChilledBuff(NPC npc, float multiplier, int durationTicks, bool syncMP = true)
        {
            chilledMultiplier = multiplier;
            npc.AddBuff(ModContent.BuffType<Chilled>(), durationTicks, true);

            if (syncMP && Main.netMode != NetmodeID.SinglePlayer)
            {
                BuffPacketHandler.CSendAddChilledBuffNPC(npc.whoAmI, multiplier, durationTicks);
            }
        }

        public override void OnHitNPC(NPC npc, NPC target, int damage, float knockback, bool crit)
        {
            target.GetGlobalNPC<PoMNPC>().lastDamageDealer = npc;
        }

        public override void ResetEffects(NPC npc)
        {
            isOnBurningAir = false;
            isIgnited = false;
            isOnShockedAir = false;
            isShocked = false;
            isOnChilledAir = false;
            isChilled = false;

            dotInstanceCollection.ResetEffects();
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            foreach (var kv in dotInstanceCollection.dotInstances)
            {
                Type type = kv.Key;
                int dps = kv.Value.dps;
                if (dps > 0)
                {
                    int debuffDamage = (int)Math.Round(dps * DamageOverTime.damageMultiplierHalfSecond);
                    if (npc.lifeRegen > 0)
                    {
                        npc.lifeRegen = 0;
                    }
                    npc.lifeRegen -= debuffDamage;

                    //TODO: this only works with buffs from this mod; could use BuffLoader.buffs
                    npc.AddBuff(mod.BuffType(type.Name), 2, true);
                }
            }
            if (isOnBurningAir && burningAirDps > 0)
            {
                int debuffDamage = (int)Math.Round(burningAirDps * DamageOverTime.damageMultiplierHalfSecond);
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= debuffDamage;
            }
            if (isIgnited && igniteDps > 0)
            {
                int debuffDamage = (int)Math.Round(igniteDps * DamageOverTime.damageMultiplierHalfSecond);
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= debuffDamage;
            }

            if (npc.lifeRegen < 0)
            {
                damage = npc.lifeRegen / -4;
            }
        }

        public override void NPCLoot(NPC npc)
        {
            Player lastDamageDealerPlayer = lastDamageDealer as Player;
            if (lastDamageDealerPlayer != null)
            {
                lastDamageDealerPlayer.GetModPlayer<PoMPlayer>().OnKillNPC(npc);
            }

            NPC lastDamageDealerNPC = lastDamageDealer as NPC;

            if (npc.lifeMax > 5 && npc.value > 0f && !npc.SpawnedFromStatue)
            {
                if (npc.boss || Main.rand.NextFloat(0, 1) < 0.15f)
                {
                    int stack = Main.rand.Next(1, 5) * (npc.boss ? 10 : 1);
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ModifierFragment"), stack);
                }
            }
        }

        public override void SetupShop(int type, Chest shop, ref int nextSlot)
        {
            if (type == NPCID.Wizard)
            {
                shop.item[nextSlot].SetDefaults(ModContent.ItemType<Items.ModifierFragment>());
                nextSlot++;
            }
        }

        public override void AI(NPC npc)
        {
            if (PathOfModifiers.Time % 60 == 0)
            {
                if (isOnBurningAir && burningAirDps < 0)
                {
                    int heal = Math.Abs(burningAirDps);
                    npc.life += heal;
                    npc.HealEffect(heal);
                }
                if (isIgnited && igniteDps < 0)
                {
                    int heal = Math.Abs(igniteDps);
                    npc.life += heal;
                    npc.HealEffect(heal);
                }
                if (npc.life > npc.lifeMax)
                {
                    npc.life = npc.lifeMax;
                }
            }
        }

        public RarityNPC rarity;

        public List<Affix> affixes;

        public int FreeAffixes => rarity.maxAffixes - affixes.Count;

        public PoMNPC()
        {
            rarity = ((PoMDataLoader.raritiesNPC?.Length ?? 0) == 0) ? new NPCNone() : PoMDataLoader.raritiesNPC[PoMDataLoader.rarityNPCMap[typeof(NPCNone)]];
            affixes = new List<Affix>();
        }

        public string GetBaseName(NPC npc) => Lang.GetNPCNameValue(npc.type);

        public void UpdateName(NPC npc)
        {
            if (rarity != null && rarity.GetType() != typeof(NPCNone))
            {
                StringBuilder sb = new StringBuilder();
                foreach (Affix affix in affixes)
                {
                    sb.AppendFormat("{0} ", affix.AddedText);
                }
                sb.AppendFormat("{0} ", GetBaseName(npc));
                sb.Remove(sb.Length - 1, 1);
                npc.GivenName = sb.ToString();
                npc.rarity = rarity.vanillaRarity;
            }
        }

        /// <summary>
        /// Completely rerolls rarity and affixes.
        /// </summary>
        public void RollNPC(NPC npc)
        {
            ClearAffixes(npc);
            rarity = PoMAffixController.RollRarity(npc);
            RollAffixes(npc);
            UpdateName(npc);
        }
        public void RollAffixes(NPC npc)
        {
            Affix newAffix;
            int freeAffixes = FreeAffixes;
            for (int i = 0; i < freeAffixes; i++)
            {
                if (i >= rarity.minAffixes && Main.rand.NextFloat(0, 1) > rarity.chanceToRollAffix)
                    break;

                newAffix = PoMAffixController.RollNewAffix(this, npc);
                if (newAffix == null)
                    break;

                AddAffix(newAffix, npc);
            }
        }

        public void AddAffix(Affix affix, NPC npc, bool clone = false)
        {
            affix.AddAffix(npc, clone);

            affixes.Add(affix);
        }
        public void RemoveAffix(Affix affix, NPC npc)
        {
            affix.RemoveAffix(npc);
            affixes.Remove(affix);
        }
        public void ClearAffixes(NPC npc)
        {
            foreach (Affix affix in affixes)
            {
                affix.RemoveAffix(npc);
            }
            affixes.Clear();
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

        public override void SetDefaults(NPC npc)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (dontRollNextNPC)
                {
                    dontRollNextNPC = false;
                }
                else
                {
                    RollNPC(npc);
                    InitializeNPC(npc);
                    UpdateName(npc);

                    if (Main.netMode == NetmodeID.Server)
                        NPCPacketHandler.SNPCSyncAffixes(npc, this);
                }
            }
        }
        public void InitializeNPC(NPC npc)
        {
            foreach (var affix in affixes)
            {
                affix.InitializeNPC(this, npc);
            }
        }

        public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        {
            return base.CanBeHitByItem(npc, player, item);
        }
        public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        {
            return base.CanBeHitByProjectile(npc, projectile);
        }
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        {
            foreach (var affix in affixes)
            {
                affix.ModifyHitByItem(npc, player, item, ref damage, ref knockback, ref crit);
            }

            ShockModifyDamageTaken(ref damage);
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            foreach (var affix in affixes)
            {
                affix.ModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);
            }

            ShockModifyDamageTaken(ref damage);
        }
        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            foreach (var affix in affixes)
            {
                affix.ModifyHitPlayer(npc, target, ref damage, ref crit);
            }

            ChillModifyDamageDealt(ref damage);
        }
        public override void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            foreach (var affix in affixes)
            {
                affix.ModifyHitNPC(npc, target, ref damage, ref knockback, ref crit);
            }

            ChillModifyDamageDealt(ref damage);
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        {
            foreach (var affix in affixes)
            {
                affix.OnHitByItem(npc, player, item, damage, knockback, crit);
            }
        }
        public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        {
            foreach (var affix in affixes)
            {
                affix.OnHitByProjectile(npc, projectile, damage, knockback, crit);
            }
        }
        public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        {
            foreach (var affix in affixes)
            {
                affix.OnHitPlayer(npc, target, damage, crit);
            }
        }

        public void NetSendAffixes(BinaryWriter writer)
        {
            try
            {
                writer.Write(PoMDataLoader.rarityNPCMap[rarity.GetType()]);

                writer.Write((byte)affixes.Count);
                Affix affix;
                for (int i = 0; i < affixes.Count; i++)
                {
                    affix = affixes[i];
                    writer.Write(PoMDataLoader.affixNPCMap[affix.GetType()]);
                    affix.NetSend(writer);
                }
            }
            catch (Exception e)
            {
                mod.Logger.Error(e.ToString());
            }
        }
        public void NetReceiveAffixes(BinaryReader reader, NPC npc)
        {
            try
            {
                int rarityIndex = reader.ReadInt32();
                rarity = PoMDataLoader.raritiesNPC[rarityIndex];

                int affixCount = reader.ReadByte();
                Affix affix;
                for (int i = 0; i < affixCount; i++)
                {
                    affix = PoMDataLoader.affixesNPC[reader.ReadInt32()].Clone();
                    affix.NetReceive(reader);
                    AddAffix(affix, npc);
                }
                UpdateName(npc);
            }
            catch (Exception e)
            {
                mod.Logger.Error(e.ToString());
            }
        }
    }
}