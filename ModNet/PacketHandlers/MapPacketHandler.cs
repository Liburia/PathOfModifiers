using Terraria.ModLoader;
using Terraria;
using System.IO;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Microsoft.Xna.Framework;
using PathOfModifiers.Tiles;
using Terraria.DataStructures;

namespace PathOfModifiers.ModNet.PacketHandlers
{
    internal class MapPacketHandler : PacketHandler
    {
        static MapPacketHandler Instance { get; set; }

        public enum PacketType
        {
            MapDeviceOpenMap,
            MapDeviceCloseMap,
            MapDeviceMapItemChanged,
        }

        public MapPacketHandler() : base(PacketHandlerType.Map)
        {
            Instance = this;
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            PacketType packetType = (PacketType)reader.ReadByte();
            switch (packetType)
            {
                case PacketType.MapDeviceOpenMap:
                    SReceiveMapDeviceOpenMap(reader);
                    break;
                case PacketType.MapDeviceCloseMap:
                    SReceiveMapDeviceCloseMap(reader);
                    break;
                case PacketType.MapDeviceMapItemChanged:
                    SReceiveMapDeviceMapItemChanged(reader);
                    break;
            }
        }
        /// <summary>
        /// Syncs all map tiles, walls and NPCs to the clients.
        /// </summary>
        public static void SSyncOpenedMap(Rectangle dimensions, bool closeMap = false)
        {
            NetMessage.SendTileRange(-1, dimensions.X - 1, dimensions.Y - 1, dimensions.Width + 2, dimensions.Height + 2);

            var mapBounds = new Rectangle(dimensions.X * 16, dimensions.Y * 16, dimensions.Width * 16, dimensions.Height * 16);
            for (int i = 0; i < Main.maxNPCs; i++)
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
        public static void CMapDeviceOpenMap(int mapDeviceID)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.MapDeviceOpenMap);
            packet.Write(mapDeviceID);
            packet.Send();
        }
        void SReceiveMapDeviceOpenMap(BinaryReader reader)
        {
            int mdID = reader.ReadInt32();
            var mapDevice = (MapDeviceTE)TileEntity.ByID[mdID];
            mapDevice.OpenMap();
        }

        public static void CMapDeviceCloseMap(int mapDeviceID)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.MapDeviceCloseMap);
            packet.Write(mapDeviceID);
            packet.Send();
        }
        void SReceiveMapDeviceCloseMap(BinaryReader reader)
        {
            int mdID = reader.ReadInt32();
            var mapDevice = (MapDeviceTE)TileEntity.ByID[mdID];
            mapDevice.OpenMap();
        }

        public static void CMapDeviceMapItemChanged(int mapDeviceID, Item item)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.MapDeviceMapItemChanged);
            packet.Write((byte)Main.myPlayer);
            packet.Write(mapDeviceID);
            ItemIO.Send(item, packet, true);
            packet.Send();
        }
        void SReceiveMapDeviceMapItemChanged(BinaryReader reader)
        {
            byte ignoreClient = reader.ReadByte();
            int mdID = reader.ReadInt32();
            var md = (MapDeviceTE)TileEntity.ByID[mdID];
            md.mapItem = ItemIO.Receive(reader, true);
            md.SendToClients(ignoreClient);
        }
    }
}
