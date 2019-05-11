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
            PathOfModifiers.Instance.Logger.Debug($"Msg Received: {Main.netMode.ToString()}/{msg.ToString()}");

            //TODO: Make into switch
            if (msg == MsgType.cSyncDataMaps)
            {
                PoMDataLoader.ReceiveDataMaps(reader);
            }
            else if (msg == MsgType.PlayerConnected)
            {
                int player = reader.ReadByte();
                ModPacket packet = PathOfModifiers.Instance.GetPacket();
                packet.Write((byte)MsgType.cSyncDataMaps);
                PoMDataLoader.SendMaps(packet);
                packet.Send(player);
            }
            else if (msg == MsgType.SyncTileEntity)
            {
                int id = reader.ReadInt32();
                bool contains = reader.ReadBoolean();
                if (contains)
                {
                    PoMTileEntity tileEntity;
                    tileEntity = (PoMTileEntity)TileEntity.Read(reader, true);
                    tileEntity.ID = id;
                    TileEntity.ByID[tileEntity.ID] = tileEntity;
                    TileEntity.ByPosition[tileEntity.Position] = tileEntity;
                    tileEntity.Sync(tileEntity.ID, whoAmI);
                }
                else
                {
                    if (TileEntity.ByID.TryGetValue(id, out TileEntity tileEntity) && tileEntity is ModTileEntity)
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
                    PathOfModifiers.Instance.Logger.Warn($"Invalid buff packet received {buffType}");
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
                    PathOfModifiers.Instance.Logger.Warn($"Invalid buff packet received {buffType}");
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
            else if (msg == MsgType.sOpenMapDeviceMap)
            {
                int mdID = reader.ReadInt32();
                var mapDevice = (MapDeviceTE)TileEntity.ByID[mdID];
                mapDevice.BeginMap();
            }
            else if (msg == MsgType.sCloseMapDeviceMap)
            {
                int mdID = reader.ReadInt32();
                var mapDevice = (MapDeviceTE)TileEntity.ByID[mdID];
                mapDevice.EndMap();
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
        public static void SyncTileEntity(int id, bool byID, TileEntity te)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.SyncTileEntity);
            packet.Write(id);
            packet.Write(byID);
            TileEntity.Write(packet, te, true);
            packet.Send();
        }
        /// <summary>
        /// Syncs all map tiles, walls and NPCs to the clients.
        /// </summary>
        public static void SyncOpenedMap(Rectangle dimensions, bool closeMap = false)
        {
            NetMessage.SendTileRange(-1, dimensions.X - 1, dimensions.Y - 1, dimensions.Width + 2, dimensions.Height + 2);

            ModPacket packet = PathOfModifiers.Instance.GetPacket();

            var mapBounds = new Rectangle(dimensions.X * 16, dimensions.Y * 16, dimensions.Width * 16, dimensions.Height * 16);
            for (int i = 0; i < 200; i++)
            {
                NPC npc = Main.npc[i];
                if (mapBounds.Intersects(npc.Hitbox))
                {
                    if (closeMap)
                    {
                        npc.active = false;
                    }
                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i);
                }
            }
            //NetMessage.CompressTileBlock();
            //NetMessage.SendData(MessageID.TileSection, -1, whoAmI, null, )
            //NetMessage.SendTileRange(-1, pos.X - 1, pos.Y - 1, size.X + 2, size.Y + 2);
        }
        /// <summary>
        /// Asks the server to open the map located in the map device.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <param name="map"></param>
        public static void OpenMapDeviceMap(int mapDeviceID)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.sOpenMapDeviceMap);
            packet.Write(mapDeviceID);
            packet.Send();
        }
        public static void CloseMapDeviceMap(int mapDeviceID)
        {
            ModPacket packet = PathOfModifiers.Instance.GetPacket();
            packet.Write((byte)MsgType.sCloseMapDeviceMap);
            packet.Write(mapDeviceID);
            packet.Send();
        }
    }

    enum MsgType
    {
        cSyncDataMaps,
        PlayerConnected,
        SyncTileEntity,
        AddDamageDoTDebuffNPC,
        AddDamageDoTDebuffPlayer,
        sOpenMapDeviceMap,
        sCloseMapDeviceMap,
    }
}
