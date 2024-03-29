using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.ModNet.PacketHandlers
{
    internal class EffectPacketHandler : PacketHandler
    {
        static EffectPacketHandler Instance { get; set; }

        public enum PacketType
        {
            SyncHeal,
            SyncHealMana,
            SyncCrit,
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
                    ReceiveSyncHeal(reader, fromWho);
                    break;
                case PacketType.SyncHealMana:
                    ReceiveSyncHealMana(reader, fromWho);
                    break;
                case PacketType.SyncCrit:
                    SReceiveSyncCrit(reader, fromWho);
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
        void ReceiveSyncHeal(BinaryReader reader, int fromWho)
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

        public static void SyncHealMana(int fromWho, int amount)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.SyncHealMana);
            packet.Write((byte)fromWho);
            packet.Write(amount);
            packet.Send();
        }
        void ReceiveSyncHealMana(BinaryReader reader, int fromWho)
        {
            int playerID = reader.ReadByte();
            int amount = reader.ReadInt32();

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.SyncHealMana);
                packet.Write((byte)playerID);
                packet.Write(amount);
                packet.Send(-1, fromWho);
            }
            else
            {
                Player player = Main.player[playerID];
                PoMEffectHelper.HealMana(player, amount);
            }
        }

        public static void CSyncCrit(Player target, int howMuch)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.SyncCrit);
            packet.Write(true);
            packet.Write((byte)target.whoAmI);
            packet.Write(howMuch);
            packet.Send();
        }
        public static void CSyncCrit(NPC target, int howMuch)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.SyncCrit);
            packet.Write(false);
            packet.Write(target.whoAmI);
            packet.Write(howMuch);
            packet.Send();
        }
        void SReceiveSyncCrit(BinaryReader reader, int fromWho)
        {
            bool isPlayer = reader.ReadBoolean();
            int targetID = isPlayer ? reader.ReadByte() : reader.ReadInt32();
            int howMuch = reader.ReadInt32();

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.SyncCrit);
                packet.Write(isPlayer);
                if (isPlayer)
                {
                    packet.Write((byte)targetID);
                }
                else
                {
                    packet.Write(targetID);
                }
                packet.Write(howMuch);
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

                PoMEffectHelper.Crit(target.position, target.width, target.height, howMuch);
            }
        }
    }
}
