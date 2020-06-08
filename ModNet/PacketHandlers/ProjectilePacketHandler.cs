using Terraria.ModLoader;
using Terraria;
using System.IO;
using Terraria.ID;

namespace PathOfModifiers.ModNet.PacketHandlers
{
    internal class ProjectilePacketHandler : PacketHandler
    {
        static ProjectilePacketHandler Instance { get; set; }

        public enum PacketType
        {
            Kill,
        }

        public ProjectilePacketHandler() : base(PacketHandlerType.Projectile)
        {
            Instance = this;
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            PacketType packetType = (PacketType)reader.ReadByte();
            switch (packetType)
            {
                case PacketType.Kill:
                    ReceiveKill(reader);
                    break;
            }
        }

        public static void CSendKill(Projectile projectile)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.Kill);
            packet.Write((byte)projectile.owner);
            packet.Write(projectile.whoAmI);
            packet.Send();
        }
        void ReceiveKill(BinaryReader reader)
        {
            if (Main.netMode == NetmodeID.Server)
            {
                int ownerID = reader.ReadByte();
                int projectileID = reader.ReadInt32();

                ModPacket packet = Instance.GetPacket((byte)PacketType.Kill);
                packet.Write(projectileID);
                packet.Send(ownerID);
            }
            else
            {
                int projectileID = reader.ReadInt32();
                Main.projectile[projectileID].Kill();
            }
        }
    }
}
