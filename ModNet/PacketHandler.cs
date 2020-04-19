using Terraria.ModLoader;
using System;
using Terraria;
using System.IO;
using Terraria.ID;

namespace PathOfModifiers.ModNet
{
    internal abstract class PacketHandler
    {
        internal byte HandlerType { get; set; }

        public abstract void HandlePacket(BinaryReader reader, int fromWho);

        protected PacketHandler(byte handlerType)
        {
            HandlerType = handlerType;
        }

        protected ModPacket GetPacket(byte packetType)
        {
            var p = PathOfModifiers.Instance.GetPacket();
            p.Write(HandlerType);
            p.Write(packetType);
            return p;
        }
    }
}
