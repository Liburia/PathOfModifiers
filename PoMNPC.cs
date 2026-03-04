using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace PathOfModifiers
{
    public class PoMNPC : GlobalNPC
    {
        public override void ModifyGlobalLoot(GlobalLoot globalLoot)
        {
            var normalDrop = new PoMGlobals.DropRate.FragmentDropScalingWithValue(
                    ItemType<Items.ModifierFragment>(),
                    PoMGlobals.DropRate.Fragment.chanceDenominator,
                    PoMGlobals.DropRate.Fragment.baseMin,
                    PoMGlobals.DropRate.Fragment.baseMax,
                    PoMGlobals.DropRate.Fragment.multiplyPerValue);
            var bossDrop = new PoMGlobals.DropRate.FragmentDropScalingWithValue(
                    ItemType<Items.ModifierFragment>(),
                    1,
                    PoMGlobals.DropRate.Fragment.baseMin,
                    PoMGlobals.DropRate.Fragment.baseMax,
                    PoMGlobals.DropRate.Fragment.multiplyPerValueBoss);
            var bossHardmodeDrop = new PoMGlobals.DropRate.FragmentDropScalingWithValue(
                    ItemType<Items.ModifierFragment>(),
                    1,
                    PoMGlobals.DropRate.Fragment.baseMin,
                    PoMGlobals.DropRate.Fragment.baseMax,
                    PoMGlobals.DropRate.Fragment.multiplyPerValueBossHardmode);
            var bossPostPlanteraDrop = new PoMGlobals.DropRate.FragmentDropScalingWithValue(
                    ItemType<Items.ModifierFragment>(),
                    1,
                    PoMGlobals.DropRate.Fragment.baseMin,
                    PoMGlobals.DropRate.Fragment.baseMax,
                    PoMGlobals.DropRate.Fragment.multiplyPerValueBossPostPlantera);

            var isBoss = new LeadingConditionRule(new Conditions.LegacyHack_IsABoss());
            var isHardmode = new LeadingConditionRule(new Conditions.IsHardmode());
            var isPostPlantera = new LeadingConditionRule(new Conditions.DownedPlantera());

            isBoss.OnFailedConditions(normalDrop);
            isPostPlantera.OnFailedConditions(isHardmode);
            isHardmode.OnFailedConditions(bossDrop);

            isBoss.OnSuccess(isPostPlantera);
            isPostPlantera.OnSuccess(bossPostPlanteraDrop);
            isHardmode.OnSuccess(bossHardmodeDrop);


            globalLoot.Add(isBoss);
        }

        public override void ModifyShop(NPCShop shop)
        {
            
            if (shop.NpcType == NPCID.Wizard)
            {
                Item item = new();
                item.SetDefaults(ItemType<Items.ModifierFragment>());
                shop.Add(item);
            }
        }
    }
}