using System.IO;
using Terraria.ModLoader;

namespace PathOfModifiers.ModNet.PacketHandlers
{
    internal class PacketHandler
    {
        internal PacketHandlerType HandlerType { get; set; }

        public virtual void HandlePacket(BinaryReader reader, int fromWho) { }

        protected PacketHandler(PacketHandlerType handlerType)
        {
            HandlerType = handlerType;
        }

        protected ModPacket GetPacket(byte packetType)
        {
            var p = PathOfModifiers.Instance.GetPacket();
            p.Write((byte)HandlerType);
            p.Write(packetType);
            return p;
        }
    }
}
