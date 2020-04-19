using Terraria.ModLoader;
using Terraria;
using System.IO;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace PathOfModifiers.ModNet.PacketHandlers
{
    internal class EffectPacketHandler : PacketHandler
    {
        static EffectPacketHandler Instance { get; set; }

        public enum PacketType
        {
            SyncHealEffect,
        }

        public EffectPacketHandler() : base(PacketHandlerType.Effect)
        {
            Instance = this;
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            PacketType packetType = (PacketType)reader.ReadByte();
            switch (packetType)
            {
                case PacketType.SyncHealEffect:
                    SReceiveSyncHealEffect(reader, fromWho);
                    break;
            }
        }

        public static void CSyncHealEffect(int fromWho, int amount)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.SyncHealEffect);
            packet.Write((byte)fromWho);
            packet.Write(amount);
            packet.Send();
        }
        void SReceiveSyncHealEffect(BinaryReader reader, int fromWho)
        {
            int playerID = reader.ReadByte();
            int amount = reader.ReadInt32();

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.SyncHealEffect);
                packet.Write((byte)playerID);
                packet.Write(amount);
                packet.Send(-1, fromWho);
            }
            else
            {
                Player player = Main.player[playerID];
                player.HealEffect(amount, false);
                for (int i = 0; i < 7; i++)
                {
                    Vector2 dustPosition = player.position + new Vector2(Main.rand.NextFloat(0, player.width), Main.rand.NextFloat(0, player.height));
                    Vector2 dustVelocity = new Vector2(0, -Main.rand.NextFloat(0.5f, 2.5f));
                    float dustScale = Main.rand.NextFloat(1f, 2.5f);
                    Dust.NewDustPerfect(dustPosition, ModContent.DustType<Dusts.HealEffect>(), dustVelocity, Scale: dustScale);
                }
            }
        }
    }
}
