using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using PathOfModifiers;
using static Terraria.ModLoader.ModContent;

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
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 5;
            Item.useTime = 5;
            Item.useStyle = 1;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<Tiles.MapBorder>();
        }

        public override void AddRecipes()
        {
            if (GetInstance<PoMConfigServer>().DisableMaps)
                return;

            CreateRecipe(4)
                .AddIngredient(ModContent.ItemType<ModifierFragment>(), 10)
                .AddTile(TileID.Anvils)
                .AddRecipeGroup(RecipeGroupID.IronBar, 1)
                .Register();
        }
    }
}
