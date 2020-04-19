using Terraria.ModLoader;
using System;
using Terraria;
using System.IO;
using Terraria.ID;

namespace PathOfModifiers.ModNet
{
    internal class ModNetHandler
    {
        public const byte buffType = 1;
        internal static BuffPacketHandler buff = new BuffPacketHandler(buffType);
        public const byte effectType = 2;
        internal static EffectPacketHandler effect = new EffectPacketHandler(effectType);
        public const byte itemType = 3;
        internal static ItemPacketHandler item = new ItemPacketHandler(itemType);
        public const byte mapType = 4;
        internal static MapPacketHandler map = new MapPacketHandler(mapType);
        public const byte modType = 5;
        internal static ModPacketHandler mod = new ModPacketHandler(modType);
        public const byte npcType = 6;
        internal static NPCPacketHandler npc = new NPCPacketHandler(npcType);
        public static void HandlePacket(BinaryReader r, int fromWho)
        {
            switch (r.ReadByte())
            {
                case buffType:
                    buff.HandlePacket(r, fromWho);
                    break;
                case effectType:
                    effect.HandlePacket(r, fromWho);
                    break;
                case itemType:
                    item.HandlePacket(r, fromWho);
                    break;
                case mapType:
                    map.HandlePacket(r, fromWho);
                    break;
                case modType:
                    mod.HandlePacket(r, fromWho);
                    break;
                case npcType:
                    npc.HandlePacket(r, fromWho);
                    break;
            }
        }
    }
}
