using Terraria.ModLoader;
using Terraria;
using System.IO;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using PathOfModifiers.Tiles;

namespace PathOfModifiers.ModNet.PacketHandlers
{
    internal class ItemPacketHandler : PacketHandler
    {
        static ItemPacketHandler Instance { get; set; }

        public enum PacketType
        {
            ModifierForgeModifiedItemChanged,
            ModifierForgeModifierItemChanged,
        }

        public ItemPacketHandler() : base(PacketHandlerType.Item)
        {
            Instance = this;
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            PacketType packetType = (PacketType)reader.ReadByte();
            switch (packetType)
            {
                case PacketType.ModifierForgeModifiedItemChanged:
                    SReceiveModifierForgeModifiedItemChanged(reader);
                    break;
                case PacketType.ModifierForgeModifierItemChanged:
                    SReceiveModifierForgeModifierItemChanged(reader);
                    break;
            }
        }

        public static void CModifierForgeModifiedItemChanged(int mapDeviceID, Item item)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.ModifierForgeModifiedItemChanged);
            packet.Write((byte)Main.myPlayer);
            packet.Write(mapDeviceID);
            ItemIO.Send(item, packet, true);
            packet.Send();
        }
        void SReceiveModifierForgeModifiedItemChanged(BinaryReader reader)
        {
            byte ignoreClient = reader.ReadByte();
            int mfID = reader.ReadInt32();
            //var mf = (ModifierForgeTE)TileEntity.ByID[mfID];
            //mf.modifiedItem = ItemIO.Receive(reader, true);
            //mf.SendToClients(ignoreClient);
        }

        public static void CModifierForgeModifierItemChanged(int mapDeviceID, Item item)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.ModifierForgeModifierItemChanged);
            packet.Write((byte)Main.myPlayer);
            packet.Write(mapDeviceID);
            ItemIO.Send(item, packet, true);
            packet.Send();
        }
        void SReceiveModifierForgeModifierItemChanged(BinaryReader reader)
        {
            byte ignoreClient = reader.ReadByte();
            int mfID = reader.ReadInt32();
            //var mf = (ModifierForgeTE)TileEntity.ByID[mfID];
            //mf.modifierItem = ItemIO.Receive(reader, true);
            //mf.SendToClients(ignoreClient);
        }
    }
}
