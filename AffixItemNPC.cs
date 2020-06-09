using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes.Items;
using PathOfModifiers.Buffs;
using PathOfModifiers.ModNet.PacketHandlers;
using PathOfModifiers.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Net;

namespace PathOfModifiers
{
    public class AffixItemNPC : GlobalNPC
    {
        public override bool InstancePerEntity => false;

        public override void NPCLoot(NPC npc)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    AffixItemPlayer affixPlayer = player.GetModPlayer<AffixItemPlayer>();
                    int droppedGold = affixPlayer.goldDropChances.Roll();
                    if (droppedGold > 0)
                    {
                        Item.NewItem(npc.position, npc.width, npc.height, ItemID.GoldCoin, droppedGold);
                    }
                }
            }
        }
    }
}