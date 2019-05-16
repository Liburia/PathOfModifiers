using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Items
{
	public class ModifierForge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Modifier Forge");
			Tooltip.SetDefault("Used to modify modifiers.");
		}
		public override void SetDefaults()
        {
            item.width = 48;
            item.height = 34;
			item.value = 500000;
			item.rare = 2;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = mod.TileType("ModifierForge");
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod, "ModifierFragment", 50);
            recipe.AddRecipeGroup("PathOfModifiers:CopperBar", 20);
            recipe.AddRecipeGroup("IronBar", 15);
            recipe.SetResult(this);
			recipe.AddRecipe();
		}
    }
}
