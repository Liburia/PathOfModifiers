using Microsoft.Xna.Framework;
using PathOfModifiers.Affixes;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System;

namespace PathOfModifiers
{
    public class PoMNPC : GlobalNPC
    {
        public override void NPCLoot(NPC npc)
        {
            if (npc.lifeMax > 5 && npc.value > 0f && !npc.SpawnedFromStatue)
            {
                if (Main.rand.NextFloat(0, 1) < 0.15f)
                {
                    int stack = Main.rand.Next(1, 5);
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ModifierFragment"), stack);
                }
            }
        }
    }
}