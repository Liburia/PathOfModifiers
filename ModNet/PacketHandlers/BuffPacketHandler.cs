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

        public enum PacketType : byte
        {
            AddBleedBuffNPC,
            AddBleedBuffPlayer,
            AddPoisonBuffNPC,
            AddPoisonBuffPlayer,
            AddMoveSpeedBuffPlayer,
            AddIgnitedBuffPlayer,
            AddIgnitedBuffNPC,
            AddShockedBuffPlayer,
            AddShockedBuffNPC,
            AddChilledBuffPlayer,
            AddChilledBuffNPC,
            AddStaticStrikeBuffPlayer,
            AddDodgeChanceBuffPlayer,
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
                case PacketType.AddBleedBuffNPC:
                    ReceiveAddBleedBuffNPC(reader, fromWho);
                    break;
                case PacketType.AddBleedBuffPlayer:
                    ReceiveAddBleedBuffPlayer(reader, fromWho);
                    break;
                case PacketType.AddPoisonBuffNPC:
                    ReceiveAddPoisonBuffNPC(reader, fromWho);
                    break;
                case PacketType.AddPoisonBuffPlayer:
                    ReceiveAddPoisonBuffPlayer(reader, fromWho);
                    break;
                case PacketType.AddMoveSpeedBuffPlayer:
                    ReceiveAddMoveSpeedBuffPlayer(reader, fromWho);
                    break;
                case PacketType.AddIgnitedBuffNPC:
                    ReceiveAddIgnitedBuffNPC(reader, fromWho);
                    break;
                case PacketType.AddIgnitedBuffPlayer:
                    ReceiveAddIgnitedBuffPlayer(reader, fromWho);
                    break;
                case PacketType.AddShockedBuffNPC:
                    ReceiveAddShockedBuffNPC(reader, fromWho);
                    break;
                case PacketType.AddShockedBuffPlayer:
                    ReceiveAddShockedBuffPlayer(reader, fromWho);
                    break;
                case PacketType.AddChilledBuffNPC:
                    ReceiveAddChilledBuffNPC(reader, fromWho);
                    break;
                case PacketType.AddChilledBuffPlayer:
                    ReceiveAddChilledBuffPlayer(reader, fromWho);
                    break;
                case PacketType.AddStaticStrikeBuffPlayer:
                    ReceiveAddStaticStrikeBuffPlayer(reader, fromWho);
                    break;
                case PacketType.AddDodgeChanceBuffPlayer:
                    ReceiveAddDodgeChanceBuffPlayer(reader, fromWho);
                    break;
            }
        }

        public static void CSendAddBleedBuffNPC(int npcID, int damage, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddBleedBuffNPC);
            packet.Write((byte)npcID);
            packet.Write(damage);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddBleedBuffNPC(BinaryReader reader, int fromWho)
        {
            byte npcID = reader.ReadByte();
            int damage = reader.ReadInt32();
            int dutaionTicks = reader.ReadInt32();

            NPC npc = Main.npc[npcID];
            BuffNPC pomNPC = npc.GetGlobalNPC<BuffNPC>();
            pomNPC.AddBleedBuff(npc, damage, dutaionTicks, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddBleedBuffNPC);
                packet.Write(npcID);
                packet.Write(damage);
                packet.Write(dutaionTicks);
                packet.Send(-1, fromWho);
            }
        }

        public static void CSendAddPoisonBuffNPC(int npcID, int damage, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddPoisonBuffNPC);
            packet.Write((byte)npcID);
            packet.Write(damage);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddPoisonBuffNPC(BinaryReader reader, int fromWho)
        {
            byte npcID = reader.ReadByte();
            int damage = reader.ReadInt32();
            int dutaionTicks = reader.ReadInt32();

            NPC npc = Main.npc[npcID];
            BuffNPC pomNPC = npc.GetGlobalNPC<BuffNPC>();
            pomNPC.AddPoisonBuff(npc, damage, dutaionTicks, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddPoisonBuffNPC);
                packet.Write(npcID);
                packet.Write(damage);
                packet.Write(dutaionTicks);
                packet.Send(-1, fromWho);
            }
        }

        public static void SendAddBleedBuffPlayer(int playerID, int damage, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddBleedBuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(damage);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddBleedBuffPlayer(BinaryReader reader, int fromWho)
        {
            byte playerID = reader.ReadByte();
            int damage = reader.ReadInt32();
            int duraionTicks = reader.ReadInt32();

            Player player = Main.player[playerID];
            player.GetModPlayer<BuffPlayer>().AddBleedBuff(player, damage, duraionTicks, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddBleedBuffPlayer);
                packet.Write(playerID);
                packet.Write(damage);
                packet.Write(duraionTicks);
                packet.Send(-1, fromWho);
            }
        }

        public static void SendAddPoisonBuffPlayer(int playerID, int damage, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddPoisonBuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(damage);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddPoisonBuffPlayer(BinaryReader reader, int fromWho)
        {
            byte playerID = reader.ReadByte();
            int damage = reader.ReadInt32();
            int duraionTicks = reader.ReadInt32();

            Player player = Main.player[playerID];
            player.GetModPlayer<BuffPlayer>().AddPoisonBuff(player, damage, duraionTicks, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddPoisonBuffPlayer);
                packet.Write(playerID);
                packet.Write(damage);
                packet.Write(duraionTicks);
                packet.Send(-1, fromWho);
            }
        }

        public static void SendAddMoveSpeedBuffPlayer(int playerID, float speedBoost, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddMoveSpeedBuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(speedBoost);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddMoveSpeedBuffPlayer(BinaryReader reader, int fromWho)
        {
            byte playerID = reader.ReadByte();
            float speedBoost = reader.ReadSingle();
            int dutaionTicks = reader.ReadInt32();

            Player player = Main.player[playerID];
            player.GetModPlayer<BuffPlayer>().AddMoveSpeedBuff(player, speedBoost, dutaionTicks, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddMoveSpeedBuffPlayer);
                packet.Write(playerID);
                packet.Write(speedBoost);
                packet.Write(dutaionTicks);
                packet.Send(-1, fromWho);
            }
        }

        public static void SendAddIgnitedBuffNPC(int npcID, int dps, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddIgnitedBuffNPC);
            packet.Write((byte)npcID);
            packet.Write(dps);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddIgnitedBuffNPC(BinaryReader reader, int fromWho)
        {
            byte npcID = reader.ReadByte();
            int dps = reader.ReadInt32();
            int dutaionTicks = reader.ReadInt32();

            NPC npc = Main.npc[npcID];
            BuffNPC pomNPC = npc.GetGlobalNPC<BuffNPC>();
            pomNPC.AddIgnitedBuff(npc, dps, dutaionTicks, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddIgnitedBuffNPC);
                packet.Write(npcID);
                packet.Write(dps);
                packet.Write(dutaionTicks);
                packet.Send(-1, fromWho);
            }
        }

        public static void SendAddIgnitedBuffPlayer(int playerID, int dps, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddIgnitedBuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(dps);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddIgnitedBuffPlayer(BinaryReader reader, int fromWho)
        {
            int playerID = reader.ReadByte();
            int dps = reader.ReadInt32();
            int time = reader.ReadInt32();

            Player player = Main.player[playerID];
            player.GetModPlayer<BuffPlayer>().AddIgnitedBuff(player, dps, time, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddIgnitedBuffPlayer);
                packet.Write((byte)playerID);
                packet.Write(dps);
                packet.Write(time);
                packet.Send(-1, fromWho);
            }
        }

        public static void SendAddShockedBuffNPC(int npcID, float multiplier, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddShockedBuffNPC);
            packet.Write((byte)npcID);
            packet.Write(multiplier);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddShockedBuffNPC(BinaryReader reader, int fromWho)
        {
            byte npcID = reader.ReadByte();
            float multiplier = reader.ReadSingle();
            int dutaionTicks = reader.ReadInt32();

            NPC npc = Main.npc[npcID];
            BuffNPC pomNPC = npc.GetGlobalNPC<BuffNPC>();
            pomNPC.AddShockedBuff(npc, multiplier, dutaionTicks, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddShockedBuffNPC);
                packet.Write(npcID);
                packet.Write(multiplier);
                packet.Write(dutaionTicks);
                packet.Send(-1, fromWho);
            }
        }

        public static void SendAddShockedBuffPlayer(int playerID, float multiplier, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddShockedBuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(multiplier);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddShockedBuffPlayer(BinaryReader reader, int fromWho)
        {
            int playerID = reader.ReadByte();
            float multiplier = reader.ReadSingle();
            int time = reader.ReadInt32();

            Player player = Main.player[playerID];
            player.GetModPlayer<BuffPlayer>().AddShockedBuff(player, multiplier, time, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddShockedBuffPlayer);
                packet.Write((byte)playerID);
                packet.Write(multiplier);
                packet.Write(time);
                packet.Send(-1, fromWho);
            }
        }

        public static void SendAddChilledBuffNPC(int npcID, float multiplier, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddChilledBuffNPC);
            packet.Write((byte)npcID);
            packet.Write(multiplier);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddChilledBuffNPC(BinaryReader reader, int fromWho)
        {
            byte npcID = reader.ReadByte();
            float multiplier = reader.ReadSingle();
            int dutaionTicks = reader.ReadInt32();

            NPC npc = Main.npc[npcID];
            BuffNPC pomNPC = npc.GetGlobalNPC<BuffNPC>();
            pomNPC.AddChilledBuff(npc, multiplier, dutaionTicks, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddChilledBuffNPC);
                packet.Write(npcID);
                packet.Write(multiplier);
                packet.Write(dutaionTicks);
                packet.Send(-1, fromWho);
            }
        }

        public static void SendAddChilledBuffPlayer(int playerID, float multiplier, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddChilledBuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(multiplier);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddChilledBuffPlayer(BinaryReader reader, int fromWho)
        {
            int playerID = reader.ReadByte();
            float multiplier = reader.ReadSingle();
            int time = reader.ReadInt32();

            Player player = Main.player[playerID];
            player.GetModPlayer<BuffPlayer>().AddChilledBuff(player, multiplier, time, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddChilledBuffPlayer);
                packet.Write((byte)playerID);
                packet.Write(multiplier);
                packet.Write(time);
                packet.Send(-1, fromWho);
            }
        }

        public static void SendAddStaticStrikeBuffPlayer(int playerID, int damage, int intervalTicks, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddStaticStrikeBuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(damage);
            packet.Write(intervalTicks);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddStaticStrikeBuffPlayer(BinaryReader reader, int fromWho)
        {
            byte playerID = reader.ReadByte();
            int damage = reader.ReadInt32();
            int intervalTicks = reader.ReadInt32();
            int dutaionTicks = reader.ReadInt32();

            Player player = Main.player[playerID];
            player.GetModPlayer<BuffPlayer>().AddStaticStrikeBuff(player, damage, intervalTicks, dutaionTicks, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddStaticStrikeBuffPlayer);
                packet.Write(playerID);
                packet.Write(damage);
                packet.Write(intervalTicks);
                packet.Write(dutaionTicks);
                packet.Send(-1, fromWho);
            }
        }

        public static void SendAddDodgeChanceBuffPlayer(int playerID, float chance, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddDodgeChanceBuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(chance);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddDodgeChanceBuffPlayer(BinaryReader reader, int fromWho)
        {
            byte playerID = reader.ReadByte();
            float chance = reader.ReadSingle();
            int duraionTicks = reader.ReadInt32();

            Player player = Main.player[playerID];
            player.GetModPlayer<BuffPlayer>().AddDodgeChanceBuff(player, chance, duraionTicks, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddDodgeChanceBuffPlayer);
                packet.Write(playerID);
                packet.Write(chance);
                packet.Write(duraionTicks);
                packet.Send(-1, fromWho);
            }
        }
    }
}
