using Terraria;
using Terraria.ID;
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
			item.width = 40;
			item.height = 40;
            item.holdStyle = 0;
			item.value = 10000;
			item.rare = 2;
            item.maxStack = 9999;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.GoldCoin, 1);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
    }
}
