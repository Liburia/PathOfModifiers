using Terraria;
using Terraria.ModLoader;

namespace PathOfModifiers.Items
{
    public class ModifierFragment : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Modifier Fragment");
            Tooltip.SetDefault("Used to modify modifiers at a modifier forge.");
        }
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.holdStyle = 0;
            Item.value = 200;
            Item.rare = 2;
            Item.maxStack = 99999;
        }
    }
}
