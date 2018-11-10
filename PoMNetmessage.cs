using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.UI.Chat;
using Terraria;
using Terraria.Utilities;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using PathOfModifiers.Affixes;
using PathOfModifiers.Rarities;
using Terraria.ID;
using Terraria.DataStructures;
using PathOfModifiers.Tiles;
using PathOfModifiers;
using PathOfModifiers.Buffs;

namespace PathOfModifiers
{
	public static class PoMNetMessage
    {
        public static void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MsgType msg = (MsgType)reader.ReadByte();
            if (PathOfModifiers.logNetwork)
                PathOfModifiers.Log($"Msg Received: {Main.netMode.ToString()}/{msg.ToString()}");

            if (msg == MsgType.SyncMaps)
            {
                PoMDataLoader.ReceiveMaps(reader);
            }
            else if (msg == MsgType.PlayerConnected)
            {
                int player = reader.ReadByte();
                ModPacket packet = PathOfModifiers.Instance.GetPacket();
                packet.Write((byte)MsgType.SyncMaps);
                PoMDataLoader.SendMaps(packet);
                packet.Send(player);
            }
            else if (msg == MsgType.SyncTEModifierForge)
            {
                int id = reader.ReadInt32();
                bool contains = reader.ReadBoolean();
                if (contains)
                {
                    TEModifierForge tileEntity;
                    tileEntity = (TEModifierForge)TileEntity.Read(reader, true);
                    tileEntity.ID = id;
                    TileEntity.ByID[tileEntity.ID] = tileEntity;
                    TileEntity.ByPosition[tileEntity.Position] = tileEntity;
                    tileEntity.Sync(tileEntity.ID, whoAmI);
                }
                else
                {
                    TileEntity tileEntity;
                    if (TileEntity.ByID.TryGetValue(id, out tileEntity) && tileEntity is ModTileEntity)
                    {
                        TileEntity.ByID.Remove(id);
                        TileEntity.ByPosition.Remove(tileEntity.Position);
                    }
                }
            }
            else if (msg == MsgType.AddDamageDoTDebuffNPC)
            {
                int npcID = reader.ReadInt32();
                int buffType = reader.ReadInt32();
                int damage = reader.ReadInt32();
                int time = reader.ReadInt32();

                DamageDoTDebuff debuff = BuffLoader.GetBuff(buffType) as DamageDoTDebuff;
                if (debuff == null)
                {
                    PathOfModifiers.Log($"PathOfModifiers: Invalid buff packet received {buffType}");
                    goto SkipMsgIf;
                }
                NPC npc = Main.npc[npcID];
                PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
                pomNPC.AddDamageDoTBuff(npc, debuff, damage, time, false);

                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = PathOfModifiers.Instance.GetPacket();
                    packet.Write((byte)MsgType.AddDamageDoTDebuffNPC);
                    packet.Write(npcID);
                    packet.Write(buffType);
                    packet.Write(damage);
                    packet.Write(time);
                    packet.Send(-1, whoAmI);
                }
            }
            else if (msg == MsgType.AddDamageDoTDebuffPlayer)
            {
                int playerID = reader.ReadInt32();
                int buffType = reader.ReadInt32();
                int damage = reader.ReadInt32();
                int time = reader.ReadInt32();

                DamageDoTDebuff debuff = BuffLoader.GetBuff(buffType) as DamageDoTDebuff;
                if (debuff == null)
                {
                    PathOfModifiers.Log($"PathOfModifiers: Invalid buff packet received {buffType}");
                    goto SkipMsgIf;
                }
                Player player = Main.player[playerID];
                PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
                pomPlayer.AddDamageDoTBuff(player, debuff, damage, time, false);

                if (Main.netMode == NetmodeID.Server)
                {
                    ModPacket packet = PathOfModifiers.Instance.GetPacket
();
                    packet.Write((byte)MsgType.AddDamageDoTDebuffPlayer);
                    packet.Write(playerID);
                    packet.Write(buffType);
                    packet.Write(damage);
                    packet.Write(time);
                    packet.Send(-1, whoAmI);
                }
            }
            else if (msg == MsgType.SyncTiles)
            {
                if (Main.netMode == NetmodeID.Server)
                {


                    //NetMessage.CompressTileBlock();
                    //NetMessage.SendData(MessageID.TileSection, -1, whoAmI, null, )
                    //NetMessage.SendTileRange(-1, pos.X - 1, pos.Y - 1, size.X + 2, size.Y + 2);
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {

                }
            }

        SkipMsgIf:;
        }

        public static void PlayerConnected(int whoAmI)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.PlayerConnected);
            packet.Write((byte)whoAmI);
            packet.Send();
        }
        public static void AddDamageDoTDebuffPlayer(int whoAmI, int buffType, int damage, int time)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.AddDamageDoTDebuffPlayer);
            packet.Write(whoAmI);
            packet.Write(buffType);
            packet.Write(damage);
            packet.Write(time);
            packet.Send();
        }
        public static void AddDamageDoTDebuffNPC(int whoAmI, int buffType, int damage, int time)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.AddDamageDoTDebuffNPC);
            packet.Write(whoAmI);
            packet.Write(buffType);
            packet.Write(damage);
            packet.Write(time);
            packet.Send();
        }
        public static void SyncTEModifierForge(int id, bool byID, TileEntity te)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.SyncTEModifierForge);
            packet.Write(id);
            packet.Write(byID);
            TileEntity.Write(packet, te, true);
            packet.Send();
        }
    }

    enum MsgType
    {
        SyncMaps,
        PlayerConnected,
        SyncTEModifierForge,
        AddDamageDoTDebuffNPC,
        AddDamageDoTDebuffPlayer,
        SyncTiles,
    }
}
