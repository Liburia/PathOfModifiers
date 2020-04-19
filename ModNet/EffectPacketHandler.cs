using Terraria.ModLoader;
using Terraria;
using System.IO;
using Terraria.ID;
using Microsoft.Xna.Framework;

namespace PathOfModifiers.ModNet
{
    internal class EffectPacketHandler : PacketHandler
    {
        public const byte syncHealEffect = 1;

        public EffectPacketHandler(byte handlerType) : base(handlerType)
        {
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case syncHealEffect:
                    SReceiveSyncHealEffect(reader, fromWho);
                    break;
            }
        }

        public void CSyncHealEffect(int fromWho, int amount)
        {
            ModPacket packet = GetPacket(syncHealEffect);
            packet.Write(fromWho);
            packet.Write(amount);
            packet.Send();
        }
        void SReceiveSyncHealEffect(BinaryReader reader, int fromWho)
        {
            //TODO: send bytes for player IDs not ints
            int playerID = reader.ReadInt32();
            int amount = reader.ReadInt32();

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket(syncHealEffect);
                packet.Write(playerID);
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
