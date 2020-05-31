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
            AddDamageDoTDebuffNPC,
            AddDamageDoTDebuffPlayer,
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
                case PacketType.AddDamageDoTDebuffNPC:
                    ReceiveAddDoTBuffNPC(reader, fromWho);
                    break;
                case PacketType.AddDamageDoTDebuffPlayer:
                    ReceiveAddDoTBuffPlayer(reader, fromWho);
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

        public static void CSendAddDoTBuffNPC(int npcID, int buffType, int damage, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddDamageDoTDebuffNPC);
            packet.Write((byte)npcID);
            packet.Write(buffType);
            packet.Write(damage);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddDoTBuffNPC(BinaryReader reader, int fromWho)
        {
            byte npcID = reader.ReadByte();
            int buffType = reader.ReadInt32();
            int damage = reader.ReadInt32();
            int dutaionTicks = reader.ReadInt32();

            DamageOverTime debuff = BuffLoader.GetBuff(buffType) as DamageOverTime;
            if (debuff == null)
            {
                PathOfModifiers.Instance.Logger.Warn($"Invalid buff type received buffType: \"{buffType}\"");
                return;
            }
            NPC npc = Main.npc[npcID];
            PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
            pomNPC.AddDoTBuff(npc, debuff, damage, dutaionTicks, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddDamageDoTDebuffNPC);
                packet.Write(npcID);
                packet.Write(buffType);
                packet.Write(damage);
                packet.Write(dutaionTicks);
                packet.Send(-1, fromWho);
            }
        }

        public static void CSendAddDoTBuffPlayer(int playerID, int buffType, int damage, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddDamageDoTDebuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(buffType);
            packet.Write(damage);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddDoTBuffPlayer(BinaryReader reader, int fromWho)
        {
            byte playerID = reader.ReadByte();
            int buffType = reader.ReadInt32();
            int damage = reader.ReadInt32();
            int duraionTicks = reader.ReadInt32();

            DamageOverTime debuff = BuffLoader.GetBuff(buffType) as DamageOverTime;
            if (debuff == null)
            {
                PathOfModifiers.Instance.Logger.Warn($"Invalid buff type received buffType: \"{buffType}\"");
                return;
            }
            Player player = Main.player[playerID];
            player.GetModPlayer<BuffPlayer>().AddDoTBuff(player, debuff, damage, duraionTicks, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddDamageDoTDebuffPlayer);
                packet.Write(playerID);
                packet.Write(buffType);
                packet.Write(damage);
                packet.Write(duraionTicks);
                packet.Send(-1, fromWho);
            }
        }

        public static void CSendAddMoveSpeedBuffPlayer(int playerID, float speedMultiplier, int dutaionTicks)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.AddMoveSpeedBuffPlayer);
            packet.Write((byte)playerID);
            packet.Write(speedMultiplier);
            packet.Write(dutaionTicks);
            packet.Send();
        }
        void ReceiveAddMoveSpeedBuffPlayer(BinaryReader reader, int fromWho)
        {
            byte playerID = reader.ReadByte();
            float speedMultiplier = reader.ReadSingle();
            int dutaionTicks = reader.ReadInt32();

            Player player = Main.player[playerID];
            player.GetModPlayer<BuffPlayer>().AddMoveSpeedBuff(player, speedMultiplier, dutaionTicks, false);

            if (Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = GetPacket((byte)PacketType.AddMoveSpeedBuffPlayer);
                packet.Write(playerID);
                packet.Write(speedMultiplier);
                packet.Write(dutaionTicks);
                packet.Send(-1, fromWho);
            }
        }

        public static void CSendAddIgnitedBuffNPC(int npcID, int dps, int dutaionTicks)
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
            PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
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

        public static void CSendAddIgnitedBuffPlayer(int playerID, int dps, int dutaionTicks)
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

        public static void CSendAddShockedBuffNPC(int npcID, float multiplier, int dutaionTicks)
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
            PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
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

        public static void CSendAddShockedBuffPlayer(int playerID, float multiplier, int dutaionTicks)
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

        public static void CSendAddChilledBuffNPC(int npcID, float multiplier, int dutaionTicks)
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
            PoMNPC pomNPC = npc.GetGlobalNPC<PoMNPC>();
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

        public static void CSendAddChilledBuffPlayer(int playerID, float multiplier, int dutaionTicks)
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

        public static void CSendAddStaticStrikeBuffPlayer(int playerID, int damage, int intervalTicks, int dutaionTicks)
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

        public static void CSendAddDodgeChanceBuffPlayer(int playerID, float chance, int dutaionTicks)
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
