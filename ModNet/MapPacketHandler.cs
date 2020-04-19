using Terraria.ModLoader;
using Terraria;
using System.IO;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using PathOfModifiers.Tiles;
using Terraria.DataStructures;

namespace PathOfModifiers.ModNet
{
    internal class MapPacketHandler : PacketHandler
    {
        public const byte mapDeviceOpenMap = 1;
        public const byte mapDeviceCloseMap = 2;
        public const byte mapDeviceMapItemChanged = 3;

        public MapPacketHandler(byte handlerType) : base(handlerType)
        {
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case mapDeviceOpenMap:
                    SReceiveMapDeviceOpenMap(reader);
                    break;
                case mapDeviceCloseMap:
                    SReceiveMapDeviceCloseMap(reader);
                    break;
                case mapDeviceMapItemChanged:
                    SReceiveMapDeviceMapItemChanged(reader);
                    break;
            }
        }
        /// <summary>
        /// Syncs all map tiles, walls and NPCs to the clients.
        /// </summary>
        public void SSyncOpenedMap(Rectangle dimensions, bool closeMap = false)
        {
            NetMessage.SendTileRange(-1, dimensions.X - 1, dimensions.Y - 1, dimensions.Width + 2, dimensions.Height + 2);

            var mapBounds = new Rectangle(dimensions.X * 16, dimensions.Y * 16, dimensions.Width * 16, dimensions.Height * 16);
            for (int i = 0; i < Main.npc.Length; i++)
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
        }
        /// <summary>
        /// Asks the server to open the map located in the map device.
        /// </summary>
        /// <param name="dimensions"></param>
        /// <param name="map"></param>
        public void CMapDeviceOpenMap(int mapDeviceID)
        {
            ModPacket packet = GetPacket(mapDeviceOpenMap);
            packet.Write(mapDeviceID);
            packet.Send();
        }
        public void SReceiveMapDeviceOpenMap(BinaryReader reader)
        {
            int mdID = reader.ReadInt32();
            var mapDevice = (MapDeviceTE)TileEntity.ByID[mdID];
            mapDevice.OpenMap();
        }
        public void CMapDeviceCloseMap(int mapDeviceID)
        {
            ModPacket packet = GetPacket(mapDeviceCloseMap);
            packet.Write(mapDeviceID);
            packet.Send();
        }
        public void SReceiveMapDeviceCloseMap(BinaryReader reader)
        {
            int mdID = reader.ReadInt32();
            var mapDevice = (MapDeviceTE)TileEntity.ByID[mdID];
            mapDevice.OpenMap();
        }
        public void CMapDeviceMapItemChanged(int mapDeviceID, Item item)
        {
            ModPacket packet = GetPacket(mapDeviceMapItemChanged);
            packet.Write((byte)Main.myPlayer);
            packet.Write(mapDeviceID);
            ItemIO.Send(item, packet, true);
            packet.Send();
        }
        public void SReceiveMapDeviceMapItemChanged(BinaryReader reader)
        {
            byte ignoreClient = reader.ReadByte();
            int mdID = reader.ReadInt32();
            var md = (MapDeviceTE)TileEntity.ByID[mdID];
            md.mapItem = ItemIO.Receive(reader, true);
            md.SendToClients(ignoreClient);
        }
    }
}
