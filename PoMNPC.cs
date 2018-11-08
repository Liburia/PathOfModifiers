using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System;
using PathOfModifiers.Buffs;
using Terraria.ID;

namespace PathOfModifiers
{
    public class PoMNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool mapNpc = false;

        public Entity lastDamageDealer;

        /// <summary>
        /// Stores the damage of the hit that procced the debuff.
        /// </summary>
        Dictionary<Type, int> damageDotDebuffDamages = new Dictionary<Type, int>();

        public bool dddDamageDotDebuff = false;

        public void AddDamageDoTBuff(NPC npc, DamageDoTDebuff buff, int damage, int time, bool syncMP = true, int ignoreClient = -1)
        {
            int dddDamage = 0;
            Type buffType = buff.GetType();
            if (damageDotDebuffDamages.TryGetValue(buffType, out dddDamage))
            {
                if (damage > dddDamage)
                    damageDotDebuffDamages[buffType] = damage;
            }
            else
            {
                damageDotDebuffDamages.Add(buffType, damage);
            }
            npc.AddBuff(buff.Type, time, true);

            if (Main.netMode != NetmodeID.SinglePlayer && syncMP)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)MsgType.AddDamageDoTDebuffNPC);
                packet.Write(npc.whoAmI);
                packet.Write(buff.Type);
                packet.Write(damage);
                packet.Write(time);
                packet.Send();
            }
        }

        public override void OnHitNPC(NPC npc, NPC target, int damage, float knockback, bool crit)
        {
            target.GetGlobalNPC<PoMNPC>().lastDamageDealer = npc;
        }

        public override void ResetEffects(NPC npc)
        {
            dddDamageDotDebuff = false;
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            int debuffDamage;
            if (dddDamageDotDebuff)
            {
                debuffDamage = (int)Math.Round(damageDotDebuffDamages[typeof(DamageDoTDebuff)] * DamageDoTDebuff.damageMultiplierHalfSecond);
                npc.lifeRegen -= debuffDamage;
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
                shop.item[nextSlot].SetDefaults(mod.ItemType<Items.ModifierFragment>());
                nextSlot++;
            }
        }

        public override bool CheckActive(NPC npc)
        {
            return !mapNpc;
        }
    }
}