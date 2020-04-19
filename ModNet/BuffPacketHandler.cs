using Terraria.ModLoader;
using Terraria;
using System.IO;
using Terraria.ID;
using PathOfModifiers.Buffs;

namespace PathOfModifiers.ModNet
{
    internal class BuffPacketHandler : PacketHandler
    {
        public const byte addDamageDoTDebuffNPC = 1;
        public const byte addDamageDoTDebuffPlayer = 2;
        public const byte addMoveSpeedBuffPlayer = 3;

        public BuffPacketHandler(byte handlerType) : base(handlerType)
        {
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case addDamageDoTDebuffNPC:
                    ReceiveAddDamageDoTDebuffNPC(reader, fromWho);
                    break;
                case addDamageDoTDebuffPlayer:
                    ReceiveAddDamageDoTDebuffPlayer(reader, fromWho);
                    break;
                case addMoveSpeedBuffPlayer:
                    ReceiveAddMoveSpeedBuffPlayer(reader, fromWho);
                    break;
            }
        }

        public void CSendAddDamageDoTDebuffNPC(int npcID, int buffType, int damage, int time)
        {
            ModPacket packet = GetPacket(addDamageDoTDebuffNPC);
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

            DamageDoTDebuff debuff = BuffLoader.GetBuff(buffType) as DamageDoTDebuff;
            if (debuff == null)
            {
                PathOfModifiers.Instance.Logger.Warn($"Invalid buff packet received {buffType}");
                return;
            }
            NPC npc = Main.npc[npcID];
            PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
            pomNPC.AddDamageDoTBuff(npc, debuff, damage, time, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket(addDamageDoTDebuffNPC);
                packet.Write(npcID);
                packet.Write(buffType);
                packet.Write(damage);
                packet.Write(time);
                packet.Send(-1, fromWho);
            }
        }

        public void CSendAddDamageDoTDebuffPlayer(int playerID, int buffType, int damage, int time)
        {
            ModPacket packet = GetPacket(addDamageDoTDebuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(buffType);
            packet.Write(damage);
            packet.Write(time);
            packet.Send();
        }
        void ReceiveAddDamageDoTDebuffPlayer(BinaryReader reader, int fromWho)
        {
            int playerID = reader.ReadInt32();
            int buffType = reader.ReadInt32();
            int damage = reader.ReadInt32();
            int time = reader.ReadInt32();

            DamageDoTDebuff debuff = BuffLoader.GetBuff(buffType) as DamageDoTDebuff;
            if (debuff == null)
            {
                PathOfModifiers.Instance.Logger.Warn($"Invalid buff packet received {buffType}");
                return;
            }
            Player player = Main.player[playerID];
            PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
            pomPlayer.AddDamageDoTBuff(player, debuff, damage, time, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket(addDamageDoTDebuffPlayer);
                packet.Write(playerID);
                packet.Write(buffType);
                packet.Write(damage);
                packet.Write(time);
                packet.Send(-1, fromWho);
            }
        }

        public void CSendAddMoveSpeedBuffPlayer(int playerID, float speedMultiplier, int time)
        {
            ModPacket packet = GetPacket(addMoveSpeedBuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(speedMultiplier);
            packet.Write(time);
            packet.Send();
        }
        void ReceiveAddMoveSpeedBuffPlayer(BinaryReader reader, int fromWho)
        {
            int playerID = reader.ReadInt32();
            float speedMultiplier = reader.ReadInt32();
            int time = reader.ReadInt32();

            Player player = Main.player[playerID];
            PoMPlayer pomPlayer = player.GetModPlayer<PoMPlayer>();
            pomPlayer.AddMoveSpeedBuff(player, speedMultiplier, time, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket(addMoveSpeedBuffPlayer);
                packet.Write(playerID);
                packet.Write(speedMultiplier);
                packet.Write(time);
                packet.Send(-1, fromWho);
            }
        }
    }
}
