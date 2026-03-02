using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Items
{
    public class ModifierFragment : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.holdStyle = 0;
            Item.value = 50;
            Item.rare = ItemRarityID.Green;
            Item.maxStack = 99999;
            Item.shopCustomPrice = 200;
        }

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 9999;
        }
    }
}
