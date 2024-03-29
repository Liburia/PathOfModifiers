using PathOfModifiers.ModNet.PacketHandlers;
using System;
using System.IO;

namespace PathOfModifiers.ModNet
{
    public enum PacketHandlerType
    {
        Buff = 0,
        Effect,
        Item,
        Map,
        Mod,
        NPC,
        Projectile,
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
            packetHandlers[(int)PacketHandlerType.Projectile] = new ProjectilePacketHandler();
        }

        public static void HandlePacket(BinaryReader r, int fromWho)
        {
            var handler = packetHandlers[r.ReadByte()];
            handler.HandlePacket(r, fromWho);
        }
    }
}
