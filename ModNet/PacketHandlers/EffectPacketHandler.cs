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
            SyncHeal,
            SyncFullHPCrit,
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
                case PacketType.SyncHeal:
                    SReceiveSyncHeal(reader, fromWho);
                    break;
                case PacketType.SyncFullHPCrit:
                    SReceiveSyncFullHPCrit(reader, fromWho);
                    break;
            }
        }

        public static void CSyncHeal(int fromWho, int amount)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.SyncHeal);
            packet.Write((byte)fromWho);
            packet.Write(amount);
            packet.Send();
        }
        void SReceiveSyncHeal(BinaryReader reader, int fromWho)
        {
            int playerID = reader.ReadByte();
            int amount = reader.ReadInt32();

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.SyncHeal);
                packet.Write((byte)playerID);
                packet.Write(amount);
                packet.Send(-1, fromWho);
            }
            else
            {
                Player player = Main.player[playerID];
                PoMEffectHelper.Heal(player, amount);
            }
        }

        public static void CSyncFullHPCrit(Player target)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.SyncFullHPCrit);
            packet.Write(true);
            packet.Write((byte)target.whoAmI);
            packet.Send();
        }
        public static void CSyncFullHPCrit(NPC target)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.SyncFullHPCrit);
            packet.Write(false);
            packet.Write(target.whoAmI);
            packet.Send();
        }
        void SReceiveSyncFullHPCrit(BinaryReader reader, int fromWho)
        {
            bool isPlayer = reader.ReadBoolean();
            int targetID = isPlayer ? reader.ReadByte() : reader.ReadInt32();

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.SyncFullHPCrit);
                packet.Write(isPlayer);
                if (isPlayer)
                {
                    packet.Write((byte)targetID);
                }
                else
                {
                    packet.Write(targetID);
                }
                packet.Send(-1, fromWho);
            }
            else
            {
                Entity target;
                if (isPlayer)
                {
                    target = Main.player[targetID];
                }
                else
                {
                    target = Main.npc[targetID];
                }

                PoMEffectHelper.FullHPCrit(target.position, target.width, target.height);
            }
        }
    }
}
