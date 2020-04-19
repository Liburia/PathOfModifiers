using Terraria.ModLoader;
using Terraria;
using System.IO;
using Terraria.ID;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using PathOfModifiers.Tiles;

namespace PathOfModifiers.ModNet
{
    internal class ItemPacketHandler : PacketHandler
    {
        public const byte modifierForgeModifiedItemChanged = 1;
        public const byte modifierForgeModifierItemChanged = 2;

        public ItemPacketHandler(byte handlerType) : base(handlerType)
        {
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case modifierForgeModifiedItemChanged:
                    SReceiveModifierForgeModifiedItemChanged(reader);
                    break;
                case modifierForgeModifierItemChanged:
                    SReceiveModifierForgeModifierItemChanged(reader);
                    break;
            }
        }

        public void CModifierForgeModifiedItemChanged(int mapDeviceID, Item item)
        {
            ModPacket packet = GetPacket(modifierForgeModifiedItemChanged);
            packet.Write((byte)Main.myPlayer);
            packet.Write(mapDeviceID);
            ItemIO.Send(item, packet, true);
            packet.Send();
        }
        public void SReceiveModifierForgeModifiedItemChanged(BinaryReader reader)
        {
            byte ignoreClient = reader.ReadByte();
            int mfID = reader.ReadInt32();
            var mf = (ModifierForgeTE)TileEntity.ByID[mfID];
            mf.modifiedItem = ItemIO.Receive(reader, true);
            mf.SendToClients(ignoreClient);
        }

        public void CModifierForgeModifierItemChanged(int mapDeviceID, Item item)
        {
            ModPacket packet = GetPacket(modifierForgeModifierItemChanged);
            packet.Write((byte)Main.myPlayer);
            packet.Write(mapDeviceID);
            ItemIO.Send(item, packet, true);
            packet.Send();
        }
        public void SReceiveModifierForgeModifierItemChanged(BinaryReader reader)
        {
            byte ignoreClient = reader.ReadByte();
            int mfID = reader.ReadInt32();
            var mf = (ModifierForgeTE)TileEntity.ByID[mfID];
            mf.modifierItem = ItemIO.Receive(reader, true);
            mf.SendToClients(ignoreClient);
        }
    }
}
