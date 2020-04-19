using Terraria.ModLoader;
using Terraria;
using System.IO;
using Terraria.ID;

namespace PathOfModifiers.ModNet
{
    internal class ModPacketHandler : PacketHandler
    {
        public const byte playerConnected = 1;
        public const byte syncDataMaps = 2;

        public ModPacketHandler(byte handlerType) : base(handlerType)
        {
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case playerConnected:
                    SReceivePlayerConnected(fromWho);
                    break;
                case syncDataMaps:
                    CReceiveDataMaps(reader);
                    break;
            }
        }
        public void CPlayerConnected()
        {
            ModPacket packet = GetPacket(playerConnected);
            packet.Send();
        }
        void SReceivePlayerConnected(int fromWho)
        {
            SSendDataMaps(fromWho);
        }
        void SSendDataMaps(int toWhom)
        {
            ModPacket packet = GetPacket(syncDataMaps);
            PoMDataLoader.SendMaps(packet);
            packet.Send(toWhom);
        }
        void CReceiveDataMaps(BinaryReader reader)
        {
            PoMDataLoader.ReceiveDataMaps(reader);
        }
    }
}
