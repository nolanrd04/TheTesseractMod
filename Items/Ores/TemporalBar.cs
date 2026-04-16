using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Items.Materials;
using TheTesseractMod.Items.Tesseracts;
using TheTesseractMod.Tiles;

namespace TheTesseractMod.Items.Ores
{
	public class TemporalBar : ModItem
	{
		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 9999;
			Item.value = Item.sellPrice(silver: 5);
			Item.rare = ItemRarityID.Blue;
			Item.material = true;
		}
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ModContent.ItemType<TemporalOre>(), 5);
            recipe.AddTile(ModContent.TileType<TesseractPylon>());
            recipe.Register();
        }
	}
}