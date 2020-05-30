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
        public override bool InstancePerEntity => true;


        Entity lastDamageDealer;
        int lastDamageDealerTimer;
        public Entity LastDamageDealer
        {
            get => lastDamageDealer;
            set
            {
                lastDamageDealer = value;
                lastDamageDealerTimer = 0;
            }
        }

        public override void ResetEffects(NPC npc)
        {
            if (LastDamageDealer != null)
            {
                if (lastDamageDealerTimer == 0)
                {

                    lastDamageDealer = null;
                }
                else
                {
                    lastDamageDealerTimer--;
                }
            }
        }

        public override void NPCLoot(NPC npc)
        {
            if (LastDamageDealer != null)
            {
                Player player = LastDamageDealer as Player;
                if (player != null)
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