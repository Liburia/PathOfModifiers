using Terraria.ModLoader;
using Terraria;
using System.IO;
using Terraria.ID;

namespace PathOfModifiers.ModNet.PacketHandlers
{
    internal class ModPacketHandler : PacketHandler
    {
        static ModPacketHandler Instance { get; set; }

        public enum PacketType
        {
            PlayerConnected,
            SyncDataMaps,
        }

        public ModPacketHandler() : base(PacketHandlerType.Mod)
        {
            Instance = this;
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            PacketType packetType = (PacketType)reader.ReadByte();
            switch (packetType)
            {
                case PacketType.PlayerConnected:
                    SReceivePlayerConnected(fromWho);
                    break;
                case PacketType.SyncDataMaps:
                    CReceiveDataMaps(reader);
                    break;
            }
        }
        public static void CPlayerConnected()
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.PlayerConnected);
            packet.Send();
        }
        void SReceivePlayerConnected(int fromWho)
        {
            SSendDataMaps(fromWho);
        }
        void SSendDataMaps(int toWhom)
        {
            ModPacket packet = GetPacket((byte)PacketType.SyncDataMaps);
            PoMDataLoader.SendMaps(packet);
            packet.Send(toWhom);
        }
        void CReceiveDataMaps(BinaryReader reader)
        {
            PoMDataLoader.ReceiveDataMaps(reader);
        }
    }
}
