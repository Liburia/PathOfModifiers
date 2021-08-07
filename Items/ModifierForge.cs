//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace PathOfModifiers.Items
//{
//	public class ModifierForge : ModItem
//	{
//		public override void SetStaticDefaults()
//        {
//            DisplayName.SetDefault("Modifier Forge");
//            Tooltip.SetDefault("Used to modify modifiers.");
//            Item.width = 48;
//            Item.height = 34;
//			Item.value = 500000;
//			Item.rare = 2;
//            Item.maxStack = 99;
//            Item.useTurn = true;
//            Item.autoReuse = true;
//            Item.useAnimation = 15;
//            Item.useTime = 10;
//            Item.useStyle = 1;
//            Item.consumable = true;
//            Item.createTile = ModContent.TileType<Tiles.ModifierForge>();
//        }

//		public override void AddRecipes()
//        {
//            CreateRecipe(1)
//                .AddIngredient(ModContent.ItemType<ModifierFragment>(), 50)
//                .AddRecipeGroup("PathOfModifiers:CopperBar", 20)
//                .AddRecipeGroup("IronBar", 15)
//                .AddTile(TileID.Anvils)
//                .Register();
//		}
//    }
//}
