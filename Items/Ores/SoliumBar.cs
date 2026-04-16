using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Items.Tesseracts;
using TheTesseractMod.Tiles;

namespace TheTesseractMod.Items.Ores
{
	public class SoliumBar : ModItem
	{
		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 9999;
			Item.value = Item.sellPrice(gold: 2, silver: 70);
			Item.rare = ItemRarityID.Red;
			Item.material = true;
		}
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ModContent.ItemType<SoliumOre>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
	}
}