using PathOfModifiers.Tiles;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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
            if (Main.netMode == NetmodeID.Server)
            {
                switch (packetType)
                {
                    case PacketType.ModifierForgeModifiedItemChanged:
                        SReceiveModifierForgeItemChanged(reader);
                        break;
                    case PacketType.ModifierForgeModifierItemChanged:
                        SReceiveModifierForgeFragmentChanged(reader);
                        break;
                }
            }
            else
            {
                switch (packetType)
                {
                    case PacketType.ModifierForgeModifiedItemChanged:
                        CReceiveModifierForgeItemChanged(reader);
                        break;
                    case PacketType.ModifierForgeModifierItemChanged:
                        CReceiveModifierForgeFragmentChanged(reader);
                        break;
                }
            }
        }

        public static void CModifierForgeItemChanged(int forgeID, Item item)
        {
            //System.Console.WriteLine("CModifierForgeItemChanged");
            ModPacket packet = Instance.GetPacket((byte)PacketType.ModifierForgeModifiedItemChanged);
            packet.Write((byte)Main.myPlayer);
            packet.Write(forgeID);
            ItemIO.Send(item, packet, true);
            packet.Send();
        }
        void SReceiveModifierForgeItemChanged(BinaryReader reader)
        {
            //System.Console.WriteLine("SReceiveModifierForgeItemChanged");
            byte ignoreClient = reader.ReadByte();
            int mfID = reader.ReadInt32();
            var mf = (ModifierForgeTE)TileEntity.ByID[mfID];
            var item = ItemIO.Receive(reader, true);
            mf.SetItem(item, false);
            mf.SendItemToClients(ignoreClient);
        }

        public static void SModifierForgeItemChanged(int forgeID, Item item, int ignoreClient)
        {
            //System.Console.WriteLine("SModifierForgeItemChanged");
            ModPacket packet = Instance.GetPacket((byte)PacketType.ModifierForgeModifiedItemChanged);
            packet.Write(forgeID);
            ItemIO.Send(item, packet, true);
            packet.Send(ignoreClient: ignoreClient);
        }
        void CReceiveModifierForgeItemChanged(BinaryReader reader)
        {
            //System.Console.WriteLine("CReceiveModifierForgeItemChanged");
            int mfID = reader.ReadInt32();
            var mf = (ModifierForgeTE)TileEntity.ByID[mfID];
            var item = ItemIO.Receive(reader, true);
            mf.SetItem(item, true);
        }

        public static void CModifierForgeFragmentChanged(int forgeID, Item item)
        {
            //System.Console.WriteLine("CModifierForgeFragmentChanged");
            ModPacket packet = Instance.GetPacket((byte)PacketType.ModifierForgeModifierItemChanged);
            packet.Write((byte)Main.myPlayer);
            packet.Write(forgeID);
            ItemIO.Send(item, packet, true);
            packet.Send();
        }
        void SReceiveModifierForgeFragmentChanged(BinaryReader reader)
        {
            //System.Console.WriteLine("SReceiveModifierForgeFragmentChanged");
            byte ignoreClient = reader.ReadByte();
            int mfID = reader.ReadInt32();
            var mf = (ModifierForgeTE)TileEntity.ByID[mfID];
            var item = ItemIO.Receive(reader, true);
            mf.SetFragment(item, false);
            mf.SendFragmentToClients(ignoreClient);
        }

        public static void SModifierForgeFragmentChanged(int forgeID, Item item, int ignoreClient)
        {
            //System.Console.WriteLine("SModifierForgeFragmentChanged");
            ModPacket packet = Instance.GetPacket((byte)PacketType.ModifierForgeModifierItemChanged);
            packet.Write(forgeID);
            ItemIO.Send(item, packet, true);
            packet.Send(ignoreClient: ignoreClient);
        }
        void CReceiveModifierForgeFragmentChanged(BinaryReader reader)
        {
            //System.Console.WriteLine("CReceiveModifierForgeFragmentChanged");
            int mfID = reader.ReadInt32();
            var mf = (ModifierForgeTE)TileEntity.ByID[mfID];
            var item = ItemIO.Receive(reader, true);
            mf.SetFragment(item, true);
        }
    }
}
