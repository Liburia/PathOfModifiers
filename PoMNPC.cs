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

        public override void OnHitNPC(NPC npc, NPC target, int damage, float knockback, bool crit)
        {
            target.GetGlobalNPC<PoMNPC>().lastDamageDealer = npc;
        }

        public override void ResetEffects(NPC npc)
        {
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
        }

        public override void NPCLoot(NPC npc)
        {
            Player lastDamageDealerPlayer = lastDamageDealer as Player;
            if (lastDamageDealerPlayer != null)
            {
                lastDamageDealerPlayer.GetModPlayer<AffixItemPlayer>().OnKillNPC(npc);
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

        public override void SetDefaults(NPC npc)
        {
            if (!PathOfModifiers.disableNPCModifiers && Main.netMode != NetmodeID.MultiplayerClient)
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
        }
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            foreach (var affix in affixes)
            {
                affix.ModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);
            }
        }
        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            foreach (var affix in affixes)
            {
                affix.ModifyHitPlayer(npc, target, ref damage, ref crit);
            }
        }
        public override void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            foreach (var affix in affixes)
            {
                affix.ModifyHitNPC(npc, target, ref damage, ref knockback, ref crit);
            }
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