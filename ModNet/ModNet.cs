using Terraria.ModLoader;
using System;
using Terraria;
using System.IO;
using Terraria.ID;
using PathOfModifiers.ModNet.PacketHandlers;

namespace PathOfModifiers.ModNet
{
    public enum PacketHandlerType
    {
        Buff,
        Effect,
        Item,
        Map,
        Mod,
        NPC,
    }

    internal class ModNet
    {
        internal static PacketHandler[] packetHandlers;

        public static void Initialize()
        {
            int phLength = Enum.GetNames(typeof(PacketHandlerType)).Length;
            packetHandlers = new PacketHandler[phLength];
            packetHandlers[(int)PacketHandlerType.Buff] = new BuffPacketHandler();
            packetHandlers[(int)PacketHandlerType.Effect] = new EffectPacketHandler();
            packetHandlers[(int)PacketHandlerType.Item] = new ItemPacketHandler();
            packetHandlers[(int)PacketHandlerType.Map] = new MapPacketHandler();
            packetHandlers[(int)PacketHandlerType.Mod] = new ModPacketHandler();
            packetHandlers[(int)PacketHandlerType.NPC] = new NPCPacketHandler();
        }

        public static void HandlePacket(BinaryReader r, int fromWho)
        {
            var handler = packetHandlers[r.ReadByte()];
            handler.HandlePacket(r, fromWho);
        }
    }
}
