using Terraria;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using TheTesseractMod.Tiles;

namespace TheTesseractMod.Systems
{
	public class TemporalOreSystem : ModSystem
	{
		public void BlessWorldWithTemporalOre() {
			if (Main.netMode == NetmodeID.MultiplayerClient) {
				return;
			}

			int tileType = ModContent.TileType<TemporalOreTile>();

			// Place ore in the world, similar to how hardmode ores are generated
			for (int i = 0; i < (int)(Main.maxTilesX * Main.maxTilesY * 0.00003); i++) {
				int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
				int y = WorldGen.genRand.Next((int)Main.rockLayer, Main.UnderworldLayer);

			// Only spawn in stone, ebonstone, or crimstone blocks
			if (Main.tile[x, y].TileType == TileID.Stone || 
				Main.tile[x, y].TileType == TileID.Ebonstone || 
				Main.tile[x, y].TileType == TileID.Crimtane||
				Main.tile[x, y].TileType == TileID.IceBlock || 
				Main.tile[x, y].TileType == TileID.SnowBlock ||
				Main.tile[x, y].TileType == TileID.Mud) {
				WorldGen.TileRunner(x, y, WorldGen.genRand.Next(6, 10), WorldGen.genRand.Next(6, 10), tileType);
			}
			}

			// Announce to all players
			if (Main.netMode == NetmodeID.SinglePlayer) {
				Main.NewText("Your world has been blessed with Temporal Ore!", 100, 100, 220);
			}
			else if (Main.netMode == NetmodeID.Server) {
				Terraria.Chat.ChatHelper.BroadcastChatMessage(Terraria.Localization.NetworkText.FromLiteral("Your world has been blessed with Temporal Ore!"), new Microsoft.Xna.Framework.Color(100, 100, 220));
			}
		}
	}
}
