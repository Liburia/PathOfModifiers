using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace PathOfModifiers.Affixes.Items
{
    public class ItemNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    ItemPlayer affixPlayer = player.GetModPlayer<ItemPlayer>();
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