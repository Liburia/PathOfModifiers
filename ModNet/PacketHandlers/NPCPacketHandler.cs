using Terraria.ModLoader;
using Terraria;
using System.IO;
using Terraria.ID;
using PathOfModifiers.Affixes.NPCs;

namespace PathOfModifiers.ModNet.PacketHandlers
{
    internal class NPCPacketHandler : PacketHandler
    {
        static NPCPacketHandler Instance { get; set; }

        public enum PacketType
        {
            NpcSyncAffixes,
        }

        public NPCPacketHandler() : base(PacketHandlerType.NPC)
        {
            Instance = this;
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            PacketType packetType = (PacketType)reader.ReadByte();
            switch (packetType)
            {
                case PacketType.NpcSyncAffixes:
                    CReceiveNPCSyncAffixes(reader);
                    break;
            }
        }

        public static void SNPCSyncAffixes(NPC npc, NPCNPC pomNPC)
        {
            ModPacket packet = Instance.GetPacket((byte)PacketType.NpcSyncAffixes);
            packet.Write(npc.whoAmI);
            pomNPC.NetSendAffixes(packet);
            packet.Send();
        }
        void CReceiveNPCSyncAffixes(BinaryReader reader)
        {
            var npc = Main.npc[reader.ReadInt32()];
            var pomNPC = npc.GetGlobalNPC<NPCNPC>();
            pomNPC.NetReceiveAffixes(reader, npc);
        }
    }
}
