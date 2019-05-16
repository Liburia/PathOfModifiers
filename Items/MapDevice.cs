using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace PathOfModifiers.Items
{
	public class MapDevice : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Map Device");
			Tooltip.SetDefault("Used to open other worlds.");
		}
		public override void SetDefaults()
        {
            item.width = 64;
            item.height = 64;
			item.value = 500000;
			item.rare = 2;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.createTile = mod.TileType("MapDevice");
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("PathOfModifiers:GoldBar", 25);
            recipe.AddIngredient(mod, "ModifierFragment", 50);
            recipe.AddIngredient(mod, "Map", 1);
            recipe.SetResult(this);
			recipe.AddRecipe();
		}
    }
}
