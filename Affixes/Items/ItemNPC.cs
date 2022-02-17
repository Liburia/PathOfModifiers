using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Affixes.Items
{
    public class ItemNPC : GlobalNPC
    {
        class GoldDropRule : IItemDropRule
        {
            public List<IItemDropRuleChainAttempt> ChainedRules
            {
                get;
                private set;
            }

            public GoldDropRule()
            {
                ChainedRules = new List<IItemDropRuleChainAttempt>();
            }

            public bool CanDrop(DropAttemptInfo info) => true;

            public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo) { }

            public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
            {
                ItemDropAttemptResult result = default;
                int totalGoldToDrop = 0;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player player = Main.player[i];
                    if (player.active && !player.dead)
                    {
                        ItemPlayer affixPlayer = player.GetModPlayer<ItemPlayer>();
                        totalGoldToDrop += affixPlayer.goldDropChances.Roll();
                    }
                }
                if (totalGoldToDrop > 0)
                {
                    CommonCode.DropItemFromNPC(info.npc, ItemID.GoldCoin, totalGoldToDrop);
                    result.State = ItemDropAttemptResultState.Success;
                }
                else
                {
                    result.State = ItemDropAttemptResultState.FailedRandomRoll;
                }

                return result;
            }
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            npcLoot.Add(new GoldDropRule());
        }
    }
}