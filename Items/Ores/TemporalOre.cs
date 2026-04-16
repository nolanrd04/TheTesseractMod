using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TheTesseractMod.Tiles;

namespace TheTesseractMod.Items.Ores
{
	public class TemporalOre : ModItem
	{
		public override void SetDefaults() {
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 9999;
			Item.value = Item.sellPrice(silver: 1);
			Item.rare = ItemRarityID.Blue;
			Item.material = true;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTurn = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.autoReuse = true;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<TemporalOreTile>();
		}
	}
}
