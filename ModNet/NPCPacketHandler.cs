using Terraria.ModLoader;
using Terraria;
using System.IO;
using Terraria.ID;

namespace PathOfModifiers.ModNet
{
    internal class NPCPacketHandler : PacketHandler
    {
        public const byte npcSyncAffixes = 1;
        public const byte spawnNPC = 2;

        public NPCPacketHandler(byte handlerType) : base(handlerType)
        {
        }

        public override void HandlePacket(BinaryReader reader, int fromWho)
        {
            switch (reader.ReadByte())
            {
                case npcSyncAffixes:
                    CReceiveNPCSyncAffixes(reader);
                    break;
            }
        }

        public void SNPCSyncAffixes(NPC npc, PoMNPC pomNPC)
        {
            ModPacket packet = GetPacket(npcSyncAffixes);
            packet.Write(npc.whoAmI);
            pomNPC.NetSend(packet);
            packet.Send();
        }
        void CReceiveNPCSyncAffixes(BinaryReader reader)
        {
            var npc = Main.npc[reader.ReadInt32()];
            var pomNPC = npc.GetGlobalNPC<PoMNPC>();
            pomNPC.NetReceive(reader, npc);
        }
    }
}
