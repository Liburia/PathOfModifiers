using PathOfModifiers.ModNet.PacketHandlers;
using PathOfModifiers.Rarities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace PathOfModifiers.Affixes.NPCs
{
    public class NPCNPC : GlobalNPC
    {
        /// <summary>
        /// Set to true to skip rolling rarity and affixes for the next NPC that is spawned(and has its SetStaticDefaults called)
        /// </summary>
        public static bool dontRollNextNPC = false;

        public override bool InstancePerEntity => true;

        public Entity lastDamageDealer;

        public override void OnHitNPC(NPC npc, NPC target, int damage, float knockback, bool crit)
        {
            target.GetGlobalNPC<NPCNPC>().lastDamageDealer = npc;
        }

        //public override void OnKill(NPC npc)
        //{
        //    //TODO: record last damage instance with an interface for source and check if it matches the killing blow or smth
        //    Player lastDamageDealerPlayer = lastDamageDealer as Player;
        //    if (lastDamageDealerPlayer != null)
        //    {
        //        lastDamageDealerPlayer.GetModPlayer<Affixes.Items.PoMPlayer>().OnKillNPC(npc);
        //    }

        //    NPC lastDamageDealerNPC = lastDamageDealer as NPC;
        //}

        public RarityNPC rarity;

        public List<Affix> affixes;

        public int FreeAffixes => rarity.maxAffixes - affixes.Count;

        public NPCNPC()
        {
            affixes = new List<Affix>();
        }

        public void PostLoad()
        {
            rarity = DataManager.NPC.GetRarityRef(typeof(NPCNone));
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
            rarity = DataManager.NPC.RollRarity(npc);
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

                newAffix = DataManager.NPC.RollNewAffix(this, npc);
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

        bool isContentSetup = false;
        public override void SetStaticDefaults()
        {
            isContentSetup = true;
        }
        public override void SetDefaults(NPC npc)
        {
            if (isContentSetup)
            {
                if (!GetInstance<PoMConfigServer>().DisableNPCModifiers && Main.netMode != NetmodeID.MultiplayerClient)
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
        }
        public void InitializeNPC(NPC npc)
        {
            foreach (var affix in affixes)
            {
                affix.InitializeNPC(this, npc);
            }
        }

        //public override bool? CanBeHitByItem(NPC npc, Player player, Item item)
        //{
        //    return base.CanBeHitByItem(npc, player, item);
        //}
        //public override bool? CanBeHitByProjectile(NPC npc, Projectile projectile)
        //{
        //    return base.CanBeHitByProjectile(npc, projectile);
        //}
        //public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
        //{
        //    foreach (var affix in affixes)
        //    {
        //        affix.ModifyHitByItem(npc, player, item, ref damage, ref knockback, ref crit);
        //    }
        //}
        //public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        //{
        //    foreach (var affix in affixes)
        //    {
        //        affix.ModifyHitByProjectile(npc, projectile, ref damage, ref knockback, ref crit, ref hitDirection);
        //    }
        //}
        //public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        //{
        //    foreach (var affix in affixes)
        //    {
        //        affix.ModifyHitPlayer(npc, target, ref damage, ref crit);
        //    }
        //}
        //public override void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit)
        //{
        //    foreach (var affix in affixes)
        //    {
        //        affix.ModifyHitNPC(npc, target, ref damage, ref knockback, ref crit);
        //    }
        //}
        //public override void OnHitByItem(NPC npc, Player player, Item item, int damage, float knockback, bool crit)
        //{
        //    foreach (var affix in affixes)
        //    {
        //        affix.OnHitByItem(npc, player, item, damage, knockback, crit);
        //    }
        //}
        //public override void OnHitByProjectile(NPC npc, Projectile projectile, int damage, float knockback, bool crit)
        //{
        //    foreach (var affix in affixes)
        //    {
        //        affix.OnHitByProjectile(npc, projectile, damage, knockback, crit);
        //    }
        //}
        //public override void OnHitPlayer(NPC npc, Player target, int damage, bool crit)
        //{
        //    foreach (var affix in affixes)
        //    {
        //        affix.OnHitPlayer(npc, target, damage, crit);
        //    }
        //}

        public void NetSendAffixes(BinaryWriter writer)
        {
            try
            {
                writer.Write(DataManager.NPC.GetRarityIndex(rarity.GetType()));

                writer.Write((byte)affixes.Count);
                Affix affix;
                for (int i = 0; i < affixes.Count; i++)
                {
                    affix = affixes[i];
                    writer.Write(DataManager.NPC.GetAffixIndex(affix.GetType()));
                    affix.NetSend(writer);
                }
            }
            catch (Exception e)
            {
                Mod.Logger.Error(e.ToString());
            }
        }
        public void NetReceiveAffixes(BinaryReader reader, NPC npc)
        {
            try
            {
                int rarityIndex = reader.ReadInt32();
                rarity = DataManager.NPC.GetRarityRef(rarityIndex);

                int affixCount = reader.ReadByte();
                Affix affix;
                for (int i = 0; i < affixCount; i++)
                {
                    affix = DataManager.NPC.GetNewAffix(reader.ReadInt32());
                    affix.NetReceive(reader);
                    AddAffix(affix, npc);
                }
                UpdateName(npc);
            }
            catch (Exception e)
            {
                Mod.Logger.Error(e.ToString());
            }
        }
    }
}