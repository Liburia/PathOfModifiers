using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using PathOfModifiers;

namespace PathOfModifiers.Items
{
    public class MapBorder : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Outlines the rectangle the map will generate in.");
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 12;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 5;
            item.useTime = 5;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = mod.TileType("MapBorder");
        }

        public override void AddRecipes()
        {
            if (PathOfModifiers.disableMaps)
                return;

            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ModifierFragment>(), 10);
            recipe.AddRecipeGroup("IronBar", 1);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
        }
    }
}
