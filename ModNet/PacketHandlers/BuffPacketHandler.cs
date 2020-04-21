using Terraria.ModLoader;
using Terraria;
using System.IO;
using Terraria.ID;
using PathOfModifiers.Buffs;

namespace PathOfModifiers.ModNet.PacketHandlers
{
    internal class BuffPacketHandler : PacketHandler
    {
        static BuffPacketHandler Instance { get; set; }

        public enum PacketType
        {
            AddDamageDoTDebuffNPC,
            AddDamageDoTDebuffPlayer,
            AddMoveSpeedBuffPlayer,
        }

        public BuffPacketHandler() : base(PacketHandlerType.Buff)
        {
            Instance = this;
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            PacketType packetType = (PacketType)reader.ReadByte();
            switch (packetType)
            {
                case PacketType.AddDamageDoTDebuffNPC:
                    ReceiveAddDamageDoTDebuffNPC(reader, fromWho);
                    break;
                case PacketType.AddDamageDoTDebuffPlayer:
                    ReceiveAddDamageDoTDebuffPlayer(reader, fromWho);
                    break;
                case PacketType.AddMoveSpeedBuffPlayer:
                    ReceiveAddMoveSpeedBuffPlayer(reader, fromWho);
                    break;
            }
        }

        public static void CSendAddDamageDoTDebuffNPC(int npcID, int buffType, int damage, int time)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddDamageDoTDebuffNPC);
            packet.Write(npcID);
            packet.Write(buffType);
            packet.Write(damage);
            packet.Write(time);
            packet.Send();
        }
        void ReceiveAddDamageDoTDebuffNPC(BinaryReader reader, int fromWho)
        {
            int npcID = reader.ReadInt32();
            int buffType = reader.ReadInt32();
            int damage = reader.ReadInt32();
            int time = reader.ReadInt32();

            DamageOverTime debuff = BuffLoader.GetBuff(buffType) as DamageOverTime;
            if (debuff == null)
            {
                PathOfModifiers.Instance.Logger.Warn($"Invalid buff packet received {buffType}");
                return;
            }
            NPC npc = Main.npc[npcID];
            PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
            pomNPC.AddDoTBuff(npc, debuff, damage, time, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddDamageDoTDebuffNPC);
                packet.Write(npcID);
                packet.Write(buffType);
                packet.Write(damage);
                packet.Write(time);
                packet.Send(-1, fromWho);
            }
        }

        public static void CSendAddDamageDoTDebuffPlayer(int playerID, int buffType, int damage, int time)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddDamageDoTDebuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(buffType);
            packet.Write(damage);
            packet.Write(time);
            packet.Send();
        }
        void ReceiveAddDamageDoTDebuffPlayer(BinaryReader reader, int fromWho)
        {
            int playerID = reader.ReadByte();
            int buffType = reader.ReadInt32();
            int damage = reader.ReadInt32();
            int time = reader.ReadInt32();

            DamageOverTime debuff = BuffLoader.GetBuff(buffType) as DamageOverTime;
            if (debuff == null)
            {
                PathOfModifiers.Instance.Logger.Warn($"Invalid buff packet received {buffType}");
                return;
            }
            Player player = Main.player[playerID];
            PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
            pomPlayer.AddDoTBuff(player, debuff, damage, time, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddDamageDoTDebuffPlayer);
                packet.Write((byte)playerID);
                packet.Write(buffType);
                packet.Write(damage);
                packet.Write(time);
                packet.Send(-1, fromWho);
            }
        }

        public static void CSendAddMoveSpeedBuffPlayer(int playerID, float speedMultiplier, int time)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddMoveSpeedBuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(speedMultiplier);
            packet.Write(time);
            packet.Send();
        }
        void ReceiveAddMoveSpeedBuffPlayer(BinaryReader reader, int fromWho)
        {
            int playerID = reader.ReadByte();
            float speedMultiplier = reader.ReadInt32();
            int time = reader.ReadInt32();

            Player player = Main.player[playerID];
            PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
            pomPlayer.AddMoveSpeedBuff(player, speedMultiplier, time, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddMoveSpeedBuffPlayer);
                packet.Write((byte)playerID);
                packet.Write(speedMultiplier);
                packet.Write(time);
                packet.Send(-1, fromWho);
            }
        }
    }
}
