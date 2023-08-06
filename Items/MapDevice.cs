using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace PathOfModifiers.Items
{
    public class MapDevice : ModItem
    {
        public override void SetStaticDefaults()
        {
            //TODO: DisplayName.SetDefault("Map Device");
            // Tooltip.SetDefault("Used to open other worlds.");
        }
        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.value = 500000;
            Item.rare = 2;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = 1;
            Item.consumable = true;
            //Item.createTile = ModContent.TileType<Tiles.MapDevice>();
        }

        public override void AddRecipes()
        {
            if (GetInstance<PoMConfigServer>().DisableMaps)
                return;


            CreateRecipe(1)
                .AddIngredient(ModContent.ItemType<ModifierFragment>(), 100)
                .AddIngredient(ModContent.ItemType<Map>(), 1)
                .AddRecipeGroup("PathOfModifiers:GoldBar", 25)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
